using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using UnityEngine;

namespace TankLike.Minimap
{
    public enum MinimapIconType
    {
        Player = 0, Enemy = 1, Boss = 2, Wall = 3, Gate = 4, Shop = 5, Workshop = 6, BossKey = 7, Ground = 8
    }

    public class MinimapManager : MonoBehaviour, IManager
    {
        [System.Serializable]
        public class MinimapIcon
        {
            public MinimapIconType Type;
            public Sprite Icon;
            public Color Color;
            public float Size = 1f;
        }

        [SerializeField] private Camera _minimapCamera;
        [SerializeField] private List<MinimapIcon> _icons;
        [SerializeField] private GameObject _minimapObject;

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void PositionMinimapAtRoom(Transform room, Vector2Int size)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            // position the camera at the top of the room
            _minimapCamera.transform.position = new Vector3(room.position.x, _minimapCamera.transform.position.y, room.position.z);

            // resize the camera size so that it covers the entirety of the room
            _minimapCamera.orthographicSize = Mathf.Max(size.x, size.y);
        }

        public MinimapIcon GetMinimapIconInfo(MinimapIconType type)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            return _icons.Find(i => i.Type == type);
        }

        public List<MinimapIcon> GetMinimapIcons()
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return null;
            }

            return _icons;
        }

        public void Display()
        {
            if (!IsActive)
            {
                return;
            }

            _minimapObject.SetActive(true);
        }

        public void Hide()
        {
            _minimapObject.SetActive(false);
        }
    }
}
