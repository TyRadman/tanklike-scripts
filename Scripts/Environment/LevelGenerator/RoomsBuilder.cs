using System.Collections;
using System.Collections.Generic;
using TankLike.Environment.MapMaker;
using TankLike.Utils;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    /// <summary>
    /// Builds the entire level
    /// </summary>
    public class RoomsBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class RoomAndGate
        {
            public Room Room;
            public GateInfo Gate;
        }

        [field: SerializeField] public RoomType StartRoomType { get; set; }
        [SerializeField] private int _roomsToCreateCount = 1;

        [Header("References")]
        [SerializeField] private LevelData _levelData;
        public LevelData LevelData => _levelData;
        [SerializeField] private Room _roomPrefab;
        [SerializeField] private Room _bossRoomPrefab;


        [Header("Debug")]
        [SerializeField] private bool DisplayMap = false;
        [SerializeField] private bool OffsetRooms = true;
        [SerializeField] private List<Room> _createdRooms = new List<Room>();

        private Transform _levelParent;
        private int _currectRoomsLeftToCreate;
        private List<RoomAndGate> _roomsToConnect = new List<RoomAndGate>();
        private const int GRID_SIZE = 51;
        private const int STARTING_INDEX = 25;
        private Room[,] _roomsGrid = new Room[GRID_SIZE, GRID_SIZE];
        private RoomsManager _roomsManager;

        public void SetUp()
        {
            StartRoomType = GameManager.Instance.GameData.StartRoomType;
            _roomsManager = GameManager.Instance.RoomsManager; ;
        }

        public void BuildRandomRooms()
        {
            count = 0;

            // get info about the room from some database, select maps that haven't been selected in a while, etc..
            if (_levelParent != null)
            {
                DestroyImmediate(_levelParent.gameObject);
            }

            _levelParent = new GameObject("Level").transform;
            _createdRooms = new List<Room>();
            mandatoryGates = 4;

            _currectRoomsLeftToCreate = _roomsToCreateCount;
            _roomsToConnect = new List<RoomAndGate>();
            _roomsGrid = new Room[GRID_SIZE, GRID_SIZE];
            _roomsManager.Rooms.Clear();
            CreateLevel();
            _roomsManager.SetupRooms();
        }

        private void CreateLevel()
        {
            // build the boss room first, because this will guarantee a south facing gate
            BossRoom bossRoom = (BossRoom)CreateRoom(_levelData.BossRoom, _bossRoomPrefab, Vector3.zero);
            GameManager.Instance.GameplayRoomGenerator.SetLevelCameraLimits(_levelData.BossRoom, bossRoom);
            bossRoom.name = "Boss Room";
            bossRoom.SetRoomType(RoomType.Boss);
            _roomsGrid[STARTING_INDEX, STARTING_INDEX] = bossRoom;
            bossRoom.Location = new Vector2Int(STARTING_INDEX, STARTING_INDEX);
            _currectRoomsLeftToCreate--;
            bossRoom.SetBossData(_levelData.BossData);

            // connect the boss room to the next room and set the gate connected to the boss room as a boss gate
            GateInfo gateInfo = bossRoom.GatesInfo.Gates.Find(g => g.Gate != null);
            // create the next room
            CreateRoomBranch(gateInfo, bossRoom, new Vector2Int(2, 4));
            // replace the gate with the boss gate
            GateInfo bossGate = _roomsToConnect[0].Room.GatesInfo.Gates.Find(g => g.IsConnected);
            GameManager.Instance.GameplayRoomGenerator.ReplaceGateWithBossGate(_levelData.BossGate, bossGate, _roomsToConnect[0].Room);
            _roomsToConnect[0].Room.SetRoomType(RoomType.BossGate);

            // the next room will be based on the direction of the first room created
            CreateRooms();

            _roomsManager.OpenAllRooms();
            // change the current room to another room that has a dead end
            SetShopRoom();
            SetStartRoom();
            RemoveEnemiesFromSpecialRooms();
        }

        private int count;

        private Room CreateRoom(MapTiles_SO mapToCreate, Room roomPrefab, Vector3 roomPosition)
        {
            // create an empty room
            Room room = Instantiate(roomPrefab, _levelParent);
            room.transform.position = Vector3.zero;

            if (OffsetRooms)
            {
                room.transform.position = roomPosition;
            }

            _roomsManager.AddRoom(room);
            // load a map into it
            GameManager.Instance.GameplayRoomGenerator.BuildRoom(mapToCreate, _levelData, room);
            room.BakeRoom();
            room.gameObject.SetActive(DisplayMap);
            _createdRooms.Add(room);

            return room;
        }

        private int mandatoryGates = 4;
        private void CreateRooms()
        {
            List<RoomAndGate> paths = new List<RoomAndGate>();
            _roomsToConnect.ForEach(r => paths.Add(r));
            _roomsToConnect = new List<RoomAndGate>();

            for (int i = 0; i < paths.Count; i++)
            {
                if (paths[i].Gate.IsConnected || paths[i].Gate.Gate == null)
                {
                    continue;
                }

                CreateRoomBranch(paths[i].Gate, paths[i].Room, GetGatesNumberRange(paths.Count, i, mandatoryGates--));
            }

            if (_roomsToConnect.Count > 0)
            {
                CreateRooms();
            }
        }

        private Vector2Int GetGatesNumberRange(int pathsLeft, int index, int mandatoryGates)
        {
            int minGatesNum;
            int maxGatesNum;

            if (mandatoryGates < 0)
            {
                minGatesNum = 2;
                maxGatesNum = 1;

                if (_currectRoomsLeftToCreate > pathsLeft - index)
                {
                    maxGatesNum = Random.Range(2, 1 + Mathf.Min(_currectRoomsLeftToCreate - pathsLeft - index + 1, 4));
                    minGatesNum = 2;
                }

                if (maxGatesNum == 1) minGatesNum = 1;

            }
            else
            {
                maxGatesNum = mandatoryGates > 0? mandatoryGates : 1;
                minGatesNum = mandatoryGates > 0 ? mandatoryGates : 1;
            }

            return new Vector2Int(minGatesNum, maxGatesNum);
        }

        private void CreateRoomBranch(GateInfo previousGate, Room previousRoom, Vector2Int gatesRange)
        {
            this.Log($"Creation by {previousGate.LocalDirection} of {previousRoom.name}. Gates limit: {gatesRange.x}, {gatesRange.y}", CD.DebugType.LevelGeneration);
           ///// SELECT A MAP OF TILES CONSIDERING THE SURROUNDING ALREADY EXISTING MAPS /////
            MapTiles_SO map = null;
            Vector2Int location = previousRoom.Location;

            location += previousGate.Direction == GateDirection.North ? Vector2Int.up :
                previousGate.Direction == GateDirection.South ? Vector2Int.down :
                previousGate.Direction == GateDirection.East ? Vector2Int.right : Vector2Int.left;

            NormalRoom createdRoom;
            string roomName = string.Empty;
            int gateIndex = 0;
            int rotation = 0;

            // check if there is a gate there already, if so then cache it
            if (_roomsGrid[location.x, location.y] != null)
            {
                createdRoom = (NormalRoom)_roomsGrid[location.x, location.y];
                roomName = createdRoom.name;

                // if the room already exists, then it should've taken the previous gate into account, so we set the opposite gate as the gate index
                gateIndex = ((int)previousGate.Direction / 90 + 2) % 4;
                this.Log($"{previousRoom.name} >>> {previousGate.Direction} >>> {roomName}".Color(Color.red), CD.DebugType.LevelGeneration);
            }
            else
            {
                _currectRoomsLeftToCreate--;
                roomName = $"R {count++}";
                List<int> gateIndicesList = new List<int>();
                List<int> rotationList = new List<int>();
                List<MapTiles_SO> acceptedMaps = new List<MapTiles_SO>();
                List<MapTiles_SO> pool = _levelData.MapPools.FindAll(m => m.GateCount >= gatesRange.x && m.GateCount <= gatesRange.y);
                this.Log($"Pool = {pool.Count}", CD.DebugType.LevelGeneration);

                for (int i = 0; i < pool.Count; i++)
                {
                    int previousIndex = (((int)previousGate.Direction / 90) + 2) % 4;

                    if (HasMatchingGates(pool[i], location, ref gateIndex, ref rotation, previousIndex))
                    {
                        acceptedMaps.Add(pool[i]);
                        gateIndicesList.Add(gateIndex);
                        rotationList.Add(rotation);
                    }
                }

                if (acceptedMaps.Count == 0)
                {
                    int previousIndex = (((int)previousGate.Direction / 90) + 2) % 4;

                    for (int i = 4; i > 0; i--)
                    {
                        List<MapTiles_SO> selectedGates = _levelData.MapPools.FindAll(m => m.GateCount == i);

                        if (selectedGates.Count == 0) continue;

                        MapTiles_SO selectedMap = selectedGates.RandomItem();
                        previousIndex = (((int)previousGate.Direction / 90) + 2) % 4;

                        if (HasMatchingGates(selectedMap, location, ref gateIndex, ref rotation, previousIndex))
                        {
                            acceptedMaps.Add(selectedMap);
                            gateIndicesList.Add(gateIndex);
                            rotationList.Add(rotation);
                            break;
                        }
                    }
                }

                this.Log($"{previousRoom.name} >>> {previousGate.Direction} >>> {roomName}".Color(Color.green), CD.DebugType.LevelGeneration);
                int randomIndex = Random.Range(0, acceptedMaps.Count);
                map = acceptedMaps[randomIndex];
                gateIndex = gateIndicesList[randomIndex];
                rotation = rotationList[randomIndex];

                Vector3 roomPosition = new Vector3((location.x - STARTING_INDEX) * 62, 0f, (location.y - STARTING_INDEX) * 62);

                // WE CREATE THE ROOM HERE
                createdRoom = (NormalRoom)CreateRoom(map, _roomPrefab, roomPosition);
                
                createdRoom.SetHasEnemies(true);
            }

            createdRoom.gameObject.name = roomName;
            // assign a location for the room in the grid
            _roomsGrid[location.x, location.y] = createdRoom;
            createdRoom.Location = location;

            GateInfo selectedGate = createdRoom.GatesInfo.Gates.Find(g => (int)g.Direction / 90 == gateIndex);

            if (selectedGate == null || selectedGate.IsConnected)
            {
                Debug.LogError($"No gate with local direction {gateIndex}");
                return;
            }

            createdRoom.transform.eulerAngles += Vector3.up * rotation;

            // set the camera limits
            if (map != null)
            {
                GameManager.Instance.GameplayRoomGenerator.SetLevelCameraLimits(map, createdRoom);
            }

            selectedGate.SetDirection(AddDirections((int)selectedGate.Direction, -rotation));

            // connect the gate of the new room to the gate of the previous room
            selectedGate.SetConnection(previousRoom, previousGate.Gate);
            previousGate.SetConnection(createdRoom, selectedGate.Gate);

            // for every gate that is not connected in the room, create a connected room
            foreach (GateInfo gate in createdRoom.GatesInfo.Gates)
            {
                if (gate.Gate == null) continue;

                // if the gate is already connected, then skip it
                if (!gate.IsConnected)
                {
                    // correct the directions of the room's gates after the rotations
                    gate.SetDirection(AddDirections((int)gate.Direction, -rotation));
                    _roomsToConnect.Add(new RoomAndGate() { Room = createdRoom, Gate = gate });
                }
            }
        }

        private GateDirection AddDirections(int direction1, int direction2)
        {
            return (GateDirection)((360 + direction1 + direction2) % 360);
        }

        public bool HasMatchingGates(MapTiles_SO map, Vector2Int newLocation, ref int gateIndex, ref int rotation, int previousGateIndex)
        {
            // the room to check has to have a set of gates that matches this set
            List<Surrounding> requirements = GetSurroundingType(newLocation);
            List<Surrounding> mapConnections = new List<Surrounding>();
            map.Surroundings.ForEach(s => mapConnections.Add(s.Surrounding));

            // if the required gates' or blocks' count is higher than the number of gates or blocks the room to check has, then it can't be built
            if (requirements.FindAll(r => r == Surrounding.Gate).Count > mapConnections.FindAll(r => r == Surrounding.Gate).Count ||
                requirements.FindAll(r => r == Surrounding.Block).Count > mapConnections.FindAll(r => r == Surrounding.Block).Count)
            {
                return false;
            }

            if (SurroundingsEqualBy(requirements, mapConnections, ref gateIndex, ref rotation, previousGateIndex)) return true;

            return false;
        }

        private List<Surrounding> GetSurroundingType(Vector2Int location)
        {
            Vector2Int[] directions = new Vector2Int[] { Vector2Int.right, Vector2Int.up,
                Vector2Int.left, Vector2Int.down };
            GateDirection[] gatesToCheck = new GateDirection[] { GateDirection.West, GateDirection.South,
                GateDirection.East, GateDirection.North };
            List<Surrounding> surroundings = new List<Surrounding>();

            for (int i = 0; i < 4; i++)
            {
                Vector2Int indexToCheck = location + directions[i];
                Room room = _roomsGrid[indexToCheck.x, indexToCheck.y];

                // if there is no room with the given index, then it's a none
                if (room == null) surroundings.Add(Surrounding.None);
                // if the room in check has a gate with the current direction, then set the surrounding as gate, otherwise, it's a block
                else if (room.GatesInfo.Gates.Exists(g => g.Direction == gatesToCheck[i])) surroundings.Add(Surrounding.Gate);
                else surroundings.Add(Surrounding.Block);
            }

            return surroundings;
        }

        private bool SurroundingsEqualBy(List<Surrounding> req, List<Surrounding> map, ref int gateIndex, ref int rotation, int previousGateIndex)
        {
            int loops = 4;
            List<int> gateIndices = new List<int>();
            List<int> rotations = new List<int>();

            for (int i = 0; i < loops; i++)
            {
                if (map[i] == Surrounding.Block) continue;

                bool works = true;

                for (int j = 0; j < loops; j++)
                {
                    Surrounding reqS = req[(previousGateIndex + j) % loops];
                    Surrounding reqM = map[(i + j) % loops];

                    // don't check if None because None can be either a block or a gate
                    if (reqS == Surrounding.None)
                    {
                        continue;
                    }

                    if (reqS != reqM)
                    {
                        works = false;
                        break;
                    }
                }

                if (works)
                {
                    gateIndices.Add(i);
                    rotations.Add((4 + i - previousGateIndex) % 4 * 90);
                }
            }

            if (gateIndices.Count > 0)
            {
                int index = Random.Range(0, gateIndices.Count);
                gateIndex = gateIndices[index];
                rotation = rotations[index];
                return true;
            }

            return false;
        }

        private void SetShopRoom()
        {
            if(GameManager.Instance.LevelGenerator.ShopsBuilder.IsBuildShops())
            {
                Room shopRoom = _roomsManager.Rooms.FindAll(room => room.RoomType == RoomType.Normal).RandomItem();
                shopRoom.SetRoomType(RoomType.Shop);
            }
        }

        private void SetStartRoom()
        {
            Room bossRoom = _roomsManager.Rooms[0];
            // cache the furthest room from the boss room with the gate count of one
            Room startingRoom = null;
            startingRoom = _roomsManager.Rooms
                    .Where(room => room.GatesInfo.Gates.Count(g => g.IsConnected) == 1 && room.RoomType != RoomType.Shop)
                    .OrderByDescending(r => (r.Location - bossRoom.Location).sqrMagnitude).FirstOrDefault();
            // disable enemies spawning
            startingRoom.SetRoomType(RoomType.Start);
            Room tempRoomToStartFrom = startingRoom; 

            switch (StartRoomType)
            {
                case RoomType.Shop:
                    {
                        tempRoomToStartFrom = _createdRooms.Find(r => r.RoomType == RoomType.Shop);
                        break;
                    }
                case RoomType.Start:
                    {
                        tempRoomToStartFrom = startingRoom;
                        break;
                    }
                case RoomType.BossGate:
                    {
                        tempRoomToStartFrom = _createdRooms.Find(r => r.RoomType == RoomType.BossGate);
                        break;
                    }
            }

            _roomsManager.SetCurrentRoom(tempRoomToStartFrom);
            _roomsManager.CurrentRoom.LoadRoom();
        }

        private void RemoveEnemiesFromSpecialRooms()
        {
            NormalRoom startRoom = (NormalRoom)_createdRooms.Find(r => r.RoomType == RoomType.Start);
            startRoom.SetHasEnemies(false);

            if (GameManager.Instance.LevelGenerator.ShopsBuilder.IsBuildShops())
            {
                NormalRoom shopRoom = (NormalRoom)_createdRooms.Find(r => r.RoomType == RoomType.Shop);
                shopRoom.SetHasEnemies(false);
            }
        }

        public Room[,] GetRoomsGrid()
        {
            return _roomsGrid;
        }

        public LevelData GetCurrentLevelData()
        {
            return _levelData;
        }
    }

    public enum Surrounding
    {
        None = 0, Gate = 1, Block = 2
    }
}
