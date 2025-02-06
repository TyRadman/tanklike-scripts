using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using MapMaker;
    using Cam;

    public class LevelGenerator : MonoBehaviour, IManager
    {
        [field: SerializeField] public RoomsBuilder RoomsBuilder { get; private set; }
        [field: SerializeField] public ShopsBuilder ShopsBuilder { get; private set; }

        public bool IsActive { get; set; }

        [Header("Editor")]
        public MapTiles_SO MapToBuild;
        public LevelData LevelData;
        public NormalRoom RoomReference;
        public BuildConfigs Configs;
        public LevelType LevelType;

        private CameraLimits _startingRoomCameraLimits = new CameraLimits();

        public void SetUp()
        {
            IsActive = true;

            GenerateLevel();
        }

        private void GenerateLevel()
        {
            switch (LevelType)
            {
                case LevelType.Custom:
                    GenerateCustomLevel();
                    break;
                case LevelType.Random:
                    GenerateRandomLevel();
                    break;
            }
        }

        private void GenerateCustomLevel()
        {
            CameraLimits limits = new CameraLimits();
            limits.SetValues(GameManager.Instance.RoomsManager.CurrentRoom.CameraLimits);
            limits.AddOffset(GameManager.Instance.RoomsManager.CurrentRoom.transform.position);
            GameManager.Instance.CameraManager.SetCamerasLimits(limits);
        }

        private void GenerateRandomLevel()
        {
            RoomsBuilder.SetUp();
            RoomsBuilder.BuildRandomRooms();
            GameManager.Instance.LevelMap.CreateLevelMap(RoomsBuilder.GetRoomsGrid());
            ShopsBuilder.BuildShops();
            Room startingRoom = GameManager.Instance.RoomsManager.CurrentRoom;

            // set first room camera limits
            _startingRoomCameraLimits.SetValues(startingRoom.CameraLimits);
            _startingRoomCameraLimits.AddOffset(startingRoom.transform.position);
            Room currentRoom = GameManager.Instance.RoomsManager.CurrentRoom;
            GameManager.Instance.MinimapManager.PositionMinimapAtRoom(currentRoom.transform, currentRoom.RoomDimensions);
        }

#if UNITY_EDITOR
        public void EditorGenerateLevel()
        {
            RoomsBuilder.BuildRandomRooms();
            ShopsBuilder.BuildShops();
        }
#endif

        public void Dispose()
        {
            IsActive = false;
        }
    }
}