using System.Collections;
using System.Collections.Generic;
using TankLike.Environment;
using UnityEngine;

namespace TankLike.UI.Map
{
    [System.Serializable]
    public class RoomIconData
    {
        public Room Room;
        public List<GameObject> GateIcons = new List<GameObject>();
        public GameObject RoomIcon;
        public GameObject QuestionMarkIcon;
        public bool IsRevealed = false;

        private RectTransform _rectTransform;

        public void SetUp()
        {
            GateIcons.ForEach(i => i.SetActive(false));
            RoomIcon.SetActive(false);
            QuestionMarkIcon.SetActive(false);
            _rectTransform = RoomIcon.GetComponent<RectTransform>();
        }

        public void Dispose()
        {
            Object.Destroy(RoomIcon);
            GateIcons.ForEach(g => Object.Destroy(g.gameObject));
            GateIcons.Clear();
        }

        public void Reveal()
        {
            IsRevealed = true;
            GateIcons.ForEach(i => i.SetActive(true));
            RoomIcon.SetActive(true);
        }

        public Vector3 GetRoomIconLocalPosition()
        {
            return _rectTransform.localPosition;
        }
    }
}
