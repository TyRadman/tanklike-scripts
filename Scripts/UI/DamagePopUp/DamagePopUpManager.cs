using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.DamagePopUp
{
    using TankLike.UnitControllers;
    using TankLike.Utils;

    /// <summary>
    /// Manages the creation and display of damage pop ups.
    /// </summary>
    public class DamagePopUpManager : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }

        [SerializeField] private bool _enabled;
        [SerializeField] private DamagePopUp _damagePopUpPrefab;
        [SerializeField] private Pool<DamagePopUp> _popUpsPool;
        [SerializeField] private List<DamagePopUpInfo> _popUpInfo;

        private const float OFFSET = 0.3f;


        #region IManager
        public void SetUp()
        {
            IsActive = true;

            if (!_enabled)
            {
                return;
            }

            CreatePools();
        }

        public void Dispose()
        {
            IsActive = false;

            if (!_enabled)
            {
                return;
            }

            DisposePools();
        }
        #endregion

        public void DisplayPopUp(DamagePopUpType type, int damageText, Vector3 position)
        {
            if (!_enabled)
            {
                return;
            }

            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            position = position.Add(Random.Range(-OFFSET, OFFSET), 0f, Random.Range(-OFFSET, OFFSET));
            DamagePopUp popUp = _popUpsPool.RequestObject(position, Quaternion.identity);
            popUp.gameObject.SetActive(true);
            popUp.SetUp(damageText, _popUpInfo.Find(i => i.Type == type));
        }

        #region Pool methods
        private void CreatePools()
        {
            _popUpsPool = new Pool<DamagePopUp>(CreateNewInstance, OnObjRequest,
                OnObjRelease, OnObjClear,0);
        }

        private void DisposePools()
        {
            if(_popUpsPool != null)
            {
                _popUpsPool.Clear();
                _popUpsPool = null;
            }
        }

        private DamagePopUp CreateNewInstance()
        {
            DamagePopUp obj = Instantiate(_damagePopUpPrefab);
            GameManager.Instance.SetParentToSpawnables(obj.gameObject);
            return obj;
        }

        private void OnObjRequest(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().OnRequest();
        }

        private void OnObjRelease(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().OnRelease();
        }

        private void OnObjClear(DamagePopUp obj)
        {
            obj.GetComponent<IPoolable>().Clear();
        }
        #endregion
    }

    public enum DamagePopUpType
    {
        Damage = 0, Heal = 1, Fire = 2, XP = 3
    }
}
