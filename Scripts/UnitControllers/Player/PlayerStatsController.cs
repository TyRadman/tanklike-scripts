using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using UI.HUD;

    public class PlayerStatsController : MonoBehaviour, IController, IStatModifiersController
    {
        public bool IsActive { get; private set; }


        [Header("Speed Modifiers")]
        [SerializeField] private List<SpeedStatModifier> _speedModifiers = new List<SpeedStatModifier>();
        [SerializeField] private StatModifierType _speedIncrementStatType;
        [SerializeField] private StatModifierType _speedDecrementStatType;

        private PlayerHUD _HUD;
        private StatIconReferenceDB _statIconReferenceDB;
        private StatIconReference _speedBoostIcon;
        private StatIconReference _speedReductionIcon;
        private float _lastSpeedMultiplier = 1f;

        public void SetUp(IController controller)
        {
            if(controller is PlayerComponents player)
            {
                _HUD = GameManager.Instance.HUDController.PlayerHUDs[player.PlayerIndex];
                _statIconReferenceDB = GameManager.Instance.StatIconReferenceDB;

                _speedBoostIcon = _statIconReferenceDB.GetStatIconReference(_speedIncrementStatType);
                _speedReductionIcon = _statIconReferenceDB.GetStatIconReference(_speedDecrementStatType);
            }

        }

        #region Speed modifiers
        public void AddStat(SpeedStatModifier modifier)
        {
            _speedModifiers.Add(modifier);

            CalculateSpeedModifiersValue();

            AddSpeedStatIconToHUD();
        }

        public void RemoveStat(SpeedStatModifier modifier)
        {
            _speedModifiers.Remove(modifier);

            // remove any icons that might be displayed on the HUD
            _HUD.StatModifiersDisplayer.RemoveIcon(_speedBoostIcon);
            _HUD.StatModifiersDisplayer.RemoveIcon(_speedReductionIcon);

            CalculateSpeedModifiersValue();

            AddSpeedStatIconToHUD();
        }

        private void AddSpeedStatIconToHUD()
        {
            if (_lastSpeedMultiplier > 1f)
            {
                _HUD.StatModifiersDisplayer.AddIcon(_speedBoostIcon);
            }
            else if (_lastSpeedMultiplier < 1f)
            {
                _HUD.StatModifiersDisplayer.AddIcon(_speedReductionIcon);
            }
        }

        private void CalculateSpeedModifiersValue()
        {
            _lastSpeedMultiplier = 1f;
            int loops = _speedModifiers.Count;

            for (int i = 0; i < loops; i++)
            {
                _lastSpeedMultiplier += _speedModifiers[i].SpeedMultiplier;
            }
        }

        public float GetSpeedModifierValue()
        {
            return _lastSpeedMultiplier;
        }
        #endregion

        #region IController

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {

        }

        public void Dispose()
        {
            _speedModifiers.Clear();
        }
        #endregion
    }
}
