using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers.Shields
{
    using Combat;
    using UnitControllers;

    /// <summary>
    /// A shield module that dictates what happens when the shild is impact by an ammunition.
    /// </summary>
    public abstract class ShieldOnImpactModule : ScriptableObject
    {
        public const string MENU_ROOT = Directories.PLAYERS + "Shield/";
        public const string FILE_NAME_ROOT = "ShieldModule_";

        protected TankComponents _components;
        protected Shield _shield;

        public abstract void OnImpact(Ammunition damageDealer, Vector3 direction, Vector3 impactPoint);

        public virtual void SetUp(TankComponents components, Shield shield)
        {
            _components = components;
            _shield = shield;
        }
    }
}
