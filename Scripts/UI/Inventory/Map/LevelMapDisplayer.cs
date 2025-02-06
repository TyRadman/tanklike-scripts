using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.UI.Map
{
    using Utils;
    using Environment;

    /// <summary>
    /// Responsible for drawing the map icons for the rooms.
    /// </summary>
    public class LevelMapDisplayer : MonoBehaviour, IManager
    {
        [SerializeField] private GameObject _roomIconPrefab;
        [SerializeField] private GameObject _gateIconPrefab;
        [SerializeField] private GameObject _questionMarkPrefab;
        [SerializeField] private GameObject _bossGateIcon;
        [SerializeField] private GameObject _shopIconPrefab;
        [SerializeField] private GameObject _workshopIconPrefab;
        [SerializeField] private Transform _iconsParent;
        [SerializeField] private RectTransform _playerIcon;

        private List<RoomIconData> _icons = new List<RoomIconData>();
        private Vector2Int _levelCenterPoint;
        
        private int _roomSpacing = 0;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            _icons.ForEach(i => i.Dispose());
            _icons.Clear();
        }
        #endregion

        public void CreateLevelMap(Room[,] roomsGrid)
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            _levelCenterPoint = Vector2Int.zero;
            List<Room> rooms = new List<Room>();

            // cache rooms in a list
            for (int i = 0; i < roomsGrid.GetLength(0); i++)
            {
                for (int j = 0; j < roomsGrid.GetLength(1); j++)
                {
                    Room room = roomsGrid[i, j];

                    if (room != null)
                    {
                        rooms.Add(room);
                        _levelCenterPoint += room.Location;
                    }
                }
            }

            _levelCenterPoint /= rooms.Count;

            float roomIconWidth = _roomIconPrefab.GetComponent<RectTransform>().rect.width;
            float gateIconHeight = _gateIconPrefab.GetComponent<RectTransform>().rect.height;
            _roomSpacing = (int)(roomIconWidth + gateIconHeight);

            // draw the rooms
            for (int i = 0; i < rooms.Count; i++)
            {
                GenerateRoom(rooms[i]);
            }

            // enable and reveal the active room where the player starts
            Room startRoom = GameManager.Instance.RoomsManager.CurrentRoom;
            _icons.Find(i => i.Room == startRoom).Reveal();

            // set the player's icon as the last child so that it shows in the UI
            _playerIcon.SetAsLastSibling();
        }

        private void GenerateRoom(Room room)
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            RoomIconData data = new RoomIconData();
            _icons.Add(data);
            data.Room = room;

            RectTransform icon = Instantiate(_roomIconPrefab).GetComponent<RectTransform>();

            data.RoomIcon = icon.gameObject;
            
            icon.parent = _iconsParent;
            icon.localScale = Vector3.one;
            icon.localEulerAngles = Vector3.zero;
            Vector2Int position = (room.Location - _levelCenterPoint) * _roomSpacing;
            icon.localPosition = new Vector3(position.x, position.y, 0f);
            Transform questionMark = Instantiate(_questionMarkPrefab, _iconsParent).transform;
            data.QuestionMarkIcon = questionMark.gameObject;
            questionMark.parent = icon;
            questionMark.localPosition = Vector3.zero;

            if (room.RoomType == RoomType.Shop)
            {
                Transform shopIcon = Instantiate(_shopIconPrefab, _iconsParent).transform;
                //data.QuestionMarkIcon = questionMark.gameObject;
                shopIcon.parent = icon;
                shopIcon.localPosition = Vector3.zero;
            }
            else if (room.RoomType == RoomType.Workshop)
            {
                Transform workshopIcon = Instantiate(_workshopIconPrefab, _iconsParent).transform;
                //data.QuestionMarkIcon = questionMark.gameObject;
                workshopIcon.parent = icon;
                workshopIcon.localPosition = Vector3.zero;
            }

            // draw the gates
            List<GateInfo> gates = room.GatesInfo.Gates.FindAll(g => g.IsConnected);

            for (int j = 0; j < gates.Count; j++)
            {
                RectTransform gateIcon;
                GateDirection direction = gates[j].Direction;

                if (room.RoomType == RoomType.BossGate && direction == GateDirection.North)
                {
                    gateIcon = Instantiate(_bossGateIcon, icon).GetComponent<RectTransform>();
                    gateIcon.localPosition = Vector3.zero;
                    gateIcon.localEulerAngles = Vector3.zero;

                    float angle = (int)direction - 90;
                    gateIcon.localEulerAngles = Vector3.forward * angle;
                    gateIcon.anchoredPosition = Vector3.up * gateIcon.rect.height / 2;
                    gateIcon.parent = _iconsParent;
                    //gateIcon.SetAsLastSibling();
                }
                else
                {
                    gateIcon = Instantiate(_gateIconPrefab, icon).GetComponent<RectTransform>();
                    gateIcon.localPosition = Vector3.zero;
                    gateIcon.localEulerAngles = Vector3.zero;

                    float angle = (int)direction - 90;
                    gateIcon.localEulerAngles = Vector3.forward * angle;
                    gateIcon.anchoredPosition = GetPositionFromDirection(direction) * gateIcon.rect.height * 2f;
                }

                gateIcon.parent = _iconsParent;
                gateIcon.SetAsFirstSibling();
                data.GateIcons.Add(gateIcon.gameObject);
            }

            data.SetUp();
            GameManager.Instance.RoomsManager.OnRoomEntered += RevealRoom;
        }

        private Vector2 GetPositionFromDirection(GateDirection direction)
        {
            float angleRadian = Mathf.Deg2Rad * (int)direction;
            return new Vector2(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian));
        }

        /// <summary>
        /// Called by every room when the player enters it
        /// </summary>
        public void RevealRoom(Room room)
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            // cache the icon corresponding to the active room
            RoomIconData roomIcon = _icons.Find(i => i.Room == room);

            // if the room has already been revealed, then return
            if (roomIcon.IsRevealed)
            {
                return;
            }

            roomIcon.Reveal();
        }

        public void OnMapOpened()
        {
            Helper.CheckForManagerActivity(IsActive, GetType());

            // position the player icon where they currently are
            _playerIcon.localPosition = _icons.Find(i => i.Room == GameManager.Instance.RoomsManager.CurrentRoom).GetRoomIconLocalPosition();
        }
    }
}
