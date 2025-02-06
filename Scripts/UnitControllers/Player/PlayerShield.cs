using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Shields;
    using TankLike.Utils;

    /// <summary>
    /// Controls the player's on-hit shield that acts as an invincibility frames provider after the player takes damage.
    /// </summary>
    public class PlayerShield : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }
        public bool IsShieldActivated { get; private set; } = false;

        [Header("Settings")]
        [SerializeField] private float _activationDuration = 0.5f;
        [SerializeField] private float _shieldSize = 1.3f;

        [Header("References")]
        [SerializeField] private Shield _shieldPrefab;
        [SerializeField] private Transform _parent;

        private PlayerComponents _playerComponents;
        private Shield _shield;

        public void SetUp(IController controller)
        {
            if (controller is not PlayerComponents playerComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _playerComponents = playerComponents;

            _shield = Instantiate(_shieldPrefab, _parent);

            _shield.SetUp(_playerComponents);
            
            _shield.transform.localPosition = Vector3.zero;

            _shield.SetShieldUser(_playerComponents.Alignment);
            _shield.SetSize(_shieldSize);
        }

        public void ActivateShield()
        {
            StartCoroutine(ShieldActivationRoutine());
        }

        private IEnumerator ShieldActivationRoutine()
        {
            _shield.ActivateShield();
            IsShieldActivated = true;

            yield return new WaitForSeconds(_activationDuration - Shield.FADING_DURATION);

            IsShieldActivated = false;
            _shield.DeactivateShield();
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Dispose()
        {
            if (IsShieldActivated)
            {
                StopAllCoroutines();
                _shield.DeactivateShield();
                IsShieldActivated = false;
            }
        }

        internal Transform GetShieldParent()
        {
            return _parent;
        }

        public void Restart()
        {
        }
        #endregion
    }
}
