using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Cam;
    using Environment.MapMaker;
    using Attributes;

    /// <summary>
    /// Builds an individual room
    /// </summary>
    public class GameplayRoomGenerator : MonoBehaviour, IRoomGenerator, IManager
    {
        /// <summary>
        /// The types of painters that can be used to paint the room
        /// </summary>
        [System.Flags]
        public enum PainterType
        {
            BaseTiles = 1,
            Droppers = 2,
            Grass = 4,
            Explosives = 8,
            DestructibleWalls = 16,
        }

        public bool IsActive { get; private set; }

        [SerializeField] private PainterType _typesToPerform;
        [SerializeField, InSelf] private ExplosivesPainter _explosivesPainter;
        [SerializeField, InSelf] private DroppersPainter _droppersPainter;
        [SerializeField, InSelf] private GrassPainter _grassPainter;
        [SerializeField, InSelf] private DestructibleWallsPainter _destructibleWallsPainter;
        [SerializeField, InSelf] private BaseTilesPainter _baseTilesPainter;

        private Dictionary<PainterType, RoomPainter> _roomPainters;
        private Dictionary<PainterType, RoomPainter> _bossRoomPainters;

        #region IManager
        public void SetUp()
        {
            _explosivesPainter.SetUp();
            _droppersPainter.SetUp();
            _grassPainter.SetUp();
            _destructibleWallsPainter.SetUp();
            _baseTilesPainter.SetUp();

            _roomPainters = new Dictionary<PainterType, RoomPainter>()
            {
                { PainterType.BaseTiles, _baseTilesPainter},
                { PainterType.Explosives, _explosivesPainter},
                { PainterType.Droppers, _droppersPainter},
                { PainterType.Grass, _grassPainter},
                { PainterType.DestructibleWalls, _destructibleWallsPainter},
            };

            _bossRoomPainters = new Dictionary<PainterType, RoomPainter>()
            {
                { PainterType.BaseTiles, _baseTilesPainter},
                { PainterType.Grass, _grassPainter},
            };
        }

        public void Dispose()
        {
            _explosivesPainter.Dispose();
            _droppersPainter.Dispose();
            _grassPainter.Dispose();
            _destructibleWallsPainter.Dispose();
            _baseTilesPainter.Dispose();
        }
        #endregion

        public void BuildRoom(MapTiles_SO map, LevelData level, Room room, BuildConfigs configs = null)
        {
            MapTiles_SO mapClone = Instantiate(map);
            Dictionary<PainterType, RoomPainter> _painters;

            if(room is BossRoom)
            {
                _painters = _bossRoomPainters;
            }
            else
            {
                _painters = _roomPainters;
            }

            if(_typesToPerform.HasFlag(PainterType.BaseTiles))
            {
                _baseTilesPainter.SetConfigurations(configs);
            }

            room.MapTiles = mapClone;

            foreach (var kvp in _painters)
            {
                if (_typesToPerform.HasFlag(kvp.Key))
                {
                    kvp.Value.PaintRoom(mapClone, room);
                }
            }

            // sort the gates' list
            room.GatesInfo.SortGates();
        }

        public static GateDirection GetGateDirection(int maxX, int maxY, Vector2Int tileDimension)
        {
            if (tileDimension.x == maxX) return GateDirection.East;
            else if (tileDimension.x == 0) return GateDirection.West;
            else if (tileDimension.y == maxY) return GateDirection.North;
            else return GateDirection.South;
        }

        public void SetLevelCameraLimits(MapTiles_SO map, Room room)
        {
            CameraLimits limits = new CameraLimits();

            float roomRotation = (room.transform.eulerAngles.y + 360f) % 360f;
            float width;
            float height;

            if (roomRotation is not 0f and not 180)
            {
                width = map.Size.y;
                height = map.Size.x;

            }
            else
            {
                width = map.Size.x;
                height = map.Size.y;
            }

            float leftLimit = -(width * MapMakerSelector.TILE_SIZE) / 2;
            float rightLimit = (width * MapMakerSelector.TILE_SIZE) / 2;

            float downLimit = -(height * MapMakerSelector.TILE_SIZE) / 2;
            float upLimit = (height * MapMakerSelector.TILE_SIZE) / 2;

            limits.HorizontalLimits = new Vector2(leftLimit, rightLimit);
            limits.VerticalLimits = new Vector2(downLimit, upLimit);

            limits.AddOffset(room.transform.position);
            room.SetCameraLimits(limits);
        }

        public void ReplaceGateWithBossGate(BossRoomGate bossGate, GateInfo gateToReplace, Room room)
        {
            // create the boss gate
            RoomGate gate = gateToReplace.Gate;
            bossGate = Instantiate(bossGate, gate.transform.position, gate.transform.rotation, gate.transform.parent);
            // connect the boss gate to whatever the previous gate was connected to
            bossGate.SetConnection(gate.ConnectedRoom, gate.ConnectedGate);
            gate.ConnectedGate.SetConnection(room, bossGate);
            gateToReplace.Gate = bossGate;
            // destroy the old gate
#if UNITY_EDITOR
            DestroyImmediate(gate.gameObject);
#else
            Destroy(gate.gameObject);
#endif
        }
    }

    [System.Serializable]
    public class BuildConfigs
    {
        public List<TileTag> IgnoreTileTags;
    }
}
