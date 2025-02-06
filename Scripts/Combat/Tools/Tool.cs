using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankLike.UI;
using TankLike.UnitControllers;

namespace TankLike.Combat
{
    public abstract class Tool : ScriptableObject
    {
        [SerializeField] protected string _name;
        [SerializeField] private Sprite _icon;
        [Header("Values")]
        [SerializeField] protected int _maxAmount;
        [field: SerializeField] public AbilityConstraint Constraints { get; private set; }
        [field: SerializeField] protected ToolTags Tag { set; get; }
        [SerializeField] protected float _duration;
        [SerializeField] protected float _cooldownDuration;

        protected int _currentAmount;
        protected ToolIcon _toolIcon;
        protected TankComponents _tank;
        /// <summary>
        /// Whether the special can be activated or not
        /// </summary>
        protected bool _ready = false;

        protected const string ASSET_MENU_ROOT = Directories.TOOLS;
        protected const string NAME_PREFIX = "Tool_";

        /// <summary>
        /// Called whenever the skill is added to a tank to cache, and initialize whatever there is to cache and initialize
        /// </summary>
        /// <param name="tankTransform">The tank that has this ability</param>
        public virtual void SetUp(TankComponents tank)
        {
            _currentAmount = _maxAmount;
            _ready = true;
            _tank = tank;
            SetDuration();
        }

        /// <summary>
        /// For when the special is removed 
        /// </summary>
        /// <param name="tankTransform"></param>
        public virtual void ResetValues(Transform tankTransform)
        {

        }

        public Sprite GetIcon()
        {
            return _icon;
        }

        public virtual void UseTool()
        {
            _currentAmount--;
            _ready = false;

            // if there are no more tools, then don't do the cool down part
            if (_currentAmount <= 0)
            {
                return;
            }

            _tank.StartCoroutine(CoolDownProcess());
        }

        public virtual bool CanUseTool()
        {
            return true;
        }

        public virtual void SetDuration()
        {

        }

        public bool HasEnoughAmount()
        {
            return _currentAmount > 0;
        }

        public bool IsReady()
        {
            return _ready;
        }

        public void SetToolIconUI(ToolIcon toolIcon)
        {
            _toolIcon = toolIcon;
        }

        public int GetAmount()
        {
            return _currentAmount;
        }

        public void AddAmount(int amount)
        {
            _currentAmount += amount;
        }

        public void SetAmount(int amount)
        {
            _currentAmount = amount;
        }

        public int GetMaxAmount()
        {
            return _maxAmount;
        }

        public bool HasFullAmmunition()
        {
            return _currentAmount == _maxAmount;
        }

        public ToolTags GetTag()
        {
            return Tag;
        }

        public ToolIcon GetToolIcon()
        {
            return _toolIcon;
        }

        private IEnumerator CoolDownProcess()
        {
            float time = 0f;

            while (time < _duration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f;
            _toolIcon.SetCooldownOverlayFill(1f);

            while (time < _cooldownDuration)
            {
                time += Time.deltaTime;
                _toolIcon.SetCooldownOverlayFill(1 - time / _cooldownDuration);
                yield return null;
            }

            _ready = true;
        }

        public virtual void Dispose()
        {

        }
    }
}

public enum ToolTags
{
    Mines = 0, HealthPack = 1, OmnidirectionalShots = 2, Shield = 3, IceShot = 4, SpinningOrb = 5, Summon = 6, HealField = 7, AirDrone = 8
}
