using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TankLike.Environment.LevelGeneration
{
    using System.Runtime.CompilerServices;
    using TankLike.Environment.MapMaker;
    using TankLike.Utils;

    public class BaseTilesPainter : RoomPainter
    {
        [SerializeField] private RoomGate _gateToBuild;

        private BuildConfigs _configs;

        public void SetConfigurations(BuildConfigs configs)
        {
            _configs = configs;
        }

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            List<Tile> tiles = map.Tiles;
            MapTileStyler styler = _levelData.Styler;
            room.SetRoomDimesions(map.Size);

            Transform tilesParent = new GameObject("Tiles").transform;
            tilesParent.parent = room.transform;
            Transform gatesParent = new GameObject("Gates").transform;
            gatesParent.parent = room.transform;
            Transform spawnPointsParent = new GameObject("SpawnPoints").transform;
            spawnPointsParent.parent = room.transform;

            int xDimension = map.Tiles.Max(t => t.Dimension.x) + 1;
            int yDimension = map.Tiles.Max(t => t.Dimension.y) + 1;
            float x = (-MapMakerSelector.TILE_SIZE * xDimension / 2f + MapMakerSelector.TILE_SIZE / 2) + room.transform.position.x;
            float y = (-MapMakerSelector.TILE_SIZE * yDimension / 2f + MapMakerSelector.TILE_SIZE / 2) + room.transform.position.z;
            Vector3 startingPosition = new Vector3(x, 0f, y);

            int maxXIndex = map.Tiles.OrderByDescending(t => t.Dimension.x).First().Dimension.x;

            int maxYIndex = map.Tiles.OrderByDescending(t => t.Dimension.y).First().Dimension.y;
            int[] angles = { 0, 90, 180, 270 };

            List<RoomGate> gates = new List<RoomGate>();

            for (int i = 0; i < tiles.Count; i++)
            {
                Tile currentTile = tiles[i];

                // get the corresponding tile from the styler
                GameObject tileToBuildPrefab;

                if (currentTile.Tag != TileTag.Gate)
                {
                    if (_configs != null)
                    {
                        if (_configs.IgnoreTileTags.Exists(t => t == currentTile.Tag))
                        {
                            continue;
                        }
                    }

                    if (currentTile.GetTileType() == TileType.Ground)
                    {
                        // Build a ground tile
                        tileToBuildPrefab = styler.GetGroundTile(currentTile.Tag);

                        if (currentTile.HasNeighbourOfType(TileType.Wall, tiles))
                        {
                            currentTile.NeighbouringStatus = TileNeighbouringStatus.GroundNeighbouringWall;
                        }
                        else
                        {
                            currentTile.NeighbouringStatus = TileNeighbouringStatus.MiddleGround;
                        }
                    }
                    else if (currentTile.GetTileType() == TileType.Wall)
                    {
                        // Build a wall tile
                        tileToBuildPrefab = styler.GetRandomWallTile(currentTile.Tag);

                        if (currentTile.HasNeighbourOfType(TileType.Ground, tiles))
                        {
                            currentTile.NeighbouringStatus = TileNeighbouringStatus.WallNeighouringGround;
                        }
                        else
                        {
                            currentTile.NeighbouringStatus = TileNeighbouringStatus.MiddleWall;
                        }
                    }
                    else
                    {
                        tileToBuildPrefab = styler.GetRandomTileByTag(currentTile.Tag);
                        Debug.Log("Tile is gate and it's not possible!!!");
                    }
                }
                else
                {
                    // build a ground for the gap
                    tileToBuildPrefab = _levelData.Styler.GetRandomTileByTag(TileTag.Ground);

                    RoomGate gate = Instantiate(_gateToBuild, gatesParent);

                    gates.Add(gate);

                    GateInfo info = new GateInfo()
                    {
                        Gate = gate
                    };

                    GateDirection direction = GetGateDirection(maxXIndex, maxYIndex, currentTile.Dimension);
                    info.SetDirection(direction);
                    // we set the local direction because as the room rotates during the level generation, the main direction is change, and this results in a loss of the gates' original index in relation to the room's initial direction
                    info.SetLocalDirection(direction);
                    info.Gate.Direction = info.Direction;
                    room.GatesInfo.Gates.Add(info);

                    gate.name = $"{currentTile.Dimension.x},{currentTile.Dimension.y}";
                    // set the position of the tile
                    Vector3 gatePosition = startingPosition + new Vector3(currentTile.Dimension.x, 0f, currentTile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                    gate.transform.position = gatePosition;
                    // set the tile rotation
                    gate.transform.eulerAngles += Vector3.up * currentTile.Rotation;
                }

                // create the tile as a child to the room
                if (tileToBuildPrefab == null)
                {
                    Debug.LogError($"Tile of type {currentTile.Tag} doesn't exist in {_levelData.Styler.name} styler");
                }

                // instantiate the tile
                GameObject builtTile = Instantiate(tileToBuildPrefab, tilesParent);
                // rename it
                builtTile.name = $"{currentTile.Dimension.x},{currentTile.Dimension.y}";
                // set the position of the tile
                Vector3 position = startingPosition + new Vector3(currentTile.Dimension.x, 0f, currentTile.Dimension.y) * MapMakerSelector.TILE_SIZE;
                builtTile.transform.position = position;
                // set the tile rotation
                builtTile.transform.eulerAngles += Vector3.up * currentTile.Rotation;

                // Apply random rotation and offset to the ground tile
                if (builtTile.TryGetComponent(out GroundTile groundTile))
                {
                    styler.RandomizeGroundTileVisuals(currentTile.Tag, ref groundTile);

                    float randomAngle = angles.RandomItem();

                    float randomHorizontalRotation = 0f;
                    float randomVerticalRotation = 0f;
                    float randomVerticalOffset = 0f;

                    if (styler.RandomHorizontalRotationChance.IsChanceSuccessful())
                    {
                        randomHorizontalRotation = Random.Range(styler.RandomHorizontalRotationRange.x, styler.RandomHorizontalRotationRange.y);
                    }

                    if (styler.RandomVerticalRotationChance.IsChanceSuccessful())
                    {
                        randomVerticalRotation = Random.Range(styler.RandomVerticalRotationRange.x, styler.RandomVerticalRotationRange.y);
                    }

                    if (styler.RandomVerticalOffsetChance.IsChanceSuccessful())
                    {
                        randomVerticalOffset = Random.Range(styler.RandomVerticalOffsetRange.x, styler.RandomVerticalOffsetRange.y);
                    }

                    Quaternion rotation = Quaternion.Euler(-90f + randomVerticalRotation, randomAngle + randomHorizontalRotation, 0);
                    Vector3 offset = new Vector3(0f, randomVerticalOffset, 0f);
                    groundTile.MeshRenderer.transform.SetLocalPositionAndRotation(offset, rotation);
                }

                // Apply random rotation to wall tiles
                if (builtTile.TryGetComponent(out WallTile wallTile))
                {
                    float baseRandomAngle = angles.RandomItem();
                    float topRandomAngle = angles.RandomItem();

                    wallTile.BaseTransform.localRotation = Quaternion.Euler(0f, 0f, baseRandomAngle);
                    wallTile.TopTransform.localRotation = Quaternion.Euler(0f, 0f, topRandomAngle);
                }

                if (currentTile.Overlays.Exists(o => o == DestructableTag.SpawnPoint))
                {
                    room.Spawner.SpawnPoints.AddSpawnPoint(position, spawnPointsParent);
                    continue;
                }

                if (currentTile.Overlays.Exists(o => o == DestructableTag.BossSpawnPoint))
                {
                    GameObject spawnPoint = new GameObject("Boss Spawn Point");
                    spawnPoint.transform.parent = room.transform;
                    ((BossRoom)room).SetBossSpawnPoint(spawnPoint.transform);
                    continue;
                }

                currentTile.BuiltTile = builtTile;
            }

            for (int i = 0; i < gates.Count; i++)
            {
                RoomGate gate = gates[i];

                Tile gateGraphicsTile = tiles.FindAll(t => t.BuiltTile != null).
                    OrderBy(t => 
                    Vector3.Distance(t.BuiltTile.transform.position, gate.GetGateGraphicsTransform().position)).
                    FirstOrDefault();

                gateGraphicsTile.CurrentTag = DestructableTag.GateGraphics;
            }
        }

        public static GateDirection GetGateDirection(int maxX, int maxY, Vector2Int tileDimension)
        {
            if (tileDimension.x == maxX) return GateDirection.East;
            else if (tileDimension.x == 0) return GateDirection.West;
            else if (tileDimension.y == maxY) return GateDirection.North;
            else return GateDirection.South;
        }
    }
}
