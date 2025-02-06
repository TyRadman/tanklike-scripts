using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TankLike.EditorTools
{
    using Utils;

    public class LevelGameEditorPage : BaseGameEditorPage
    {
        private PlayerTempInfoSaver _tempInfoSaver;

        public override EGameEditorPageTag PageTag()
        {
            return EGameEditorPageTag.Level;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            _data = GetData();
        }

        public override void OnGUI()
        {
            RenderHeader("Game Editor - Level Data", 30, 20, 20, true);

            RenderSection("Level", RenderLevelSection);
        }


        private void RenderLevelSection()
        {
            if (_tempInfoSaver == null)
            {
                _tempInfoSaver = Helper.FindAssetFromProjectFiles<PlayerTempInfoSaver>();
            }

            if (_tempInfoSaver == null)
            {
                Debug.LogError("No PlayerTempInfoSaver in project files");
                return;
            }

            RenderStartRoom();

            RenderSpawnShopSection();

            EditorUtility.SetDirty(_tempInfoSaver);
        }

        private void RenderStartRoom()
        {
            GUILayout.Space(10);
            
            RoomType startRoom = _data.StartRoomType;
            int roomNumber = (int)startRoom;
            string[] roomTypes = new string[System.Enum.GetValues(typeof(RoomType)).Length];

            for (int i = 0; i < roomTypes.Length; i++)
            {
                roomTypes[i] = ((RoomType)i).ToString();
            }

            GUILayout.Label("Start Room");
            roomNumber = EditorGUILayout.Popup(roomNumber, roomTypes);
            startRoom = (RoomType)roomNumber;
            _data.StartRoomType = startRoom;
        }

        private void RenderSpawnShopSection()
        {
            bool spawnShop = _data.SpawnShop;
            bool spawnWorkshop = _data.SpawnWorkshop;

            GUILayout.Label("Spawn Shop");
            spawnShop = GUILayout.Toggle(spawnShop, "");
            GUILayout.Label("Spawn Workshop");
            spawnWorkshop = GUILayout.Toggle(spawnWorkshop, "");

            _data.SpawnShop = spawnShop;
            _data.SpawnWorkshop = spawnWorkshop;
        }
    }
}
