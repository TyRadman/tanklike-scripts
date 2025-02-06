using UnityEngine;

namespace TankLike.Combat
{
    using UnitControllers;

    [System.Serializable]
    public readonly struct DamageInfo
    {
        public int Damage { get; }
        public Vector3 Direction { get; }
        public UnitComponents Instigator { get; }
        public Vector3 BulletPosition { get; }
        public Ammunition DamageDealer { get; }
        public ImpactType DamageType { get; }
        public bool DisplayPopUp { get; }

        private DamageInfo(int damage, Vector3 direction, UnitComponents instigator, Vector3 bulletPosition, Ammunition damageDealer, ImpactType damageType, bool displayPopUp)
        {
            Damage = damage;
            Direction = direction;
            Instigator = instigator;
            BulletPosition = bulletPosition;
            DamageDealer = damageDealer;
            DamageType = damageType;
            DisplayPopUp = displayPopUp;
        }

        /// <summary>
        /// Creates a new instance of DamageInfo using the Builder class.
        /// </summary>
        /// <returns></returns>
        public static Builder Create()
        {
            return new Builder();
        }

        public sealed class Builder : IBuilder<DamageInfo>
        {
            private int _damage = 0;
            private UnitComponents _instigator = null;
            private Vector3 _direction = Vector3.zero;
            private Vector3 _bulletPosition = Vector3.zero;
            private Ammunition _damageDealer = null;
            private ImpactType _damageType = ImpactType.NonExplosive;
            private bool _displayPopUp = true;

            /// <summary>
            /// Sets the damage value.
            /// </summary>
            /// <param name="damage"></param>
            /// <returns></returns>
            public Builder SetDamage(int damage)
            {
                _damage = damage;
                return this;
            }

            /// <summary>
            /// Sets the direction of the damage source at the time of impact.
            /// </summary>
            /// <param name="direction"></param>
            /// <returns></returns>
            public Builder SetDirection(Vector3 direction)
            {
                _direction = direction;
                return this;
            }

            /// <summary>
            /// Sets the instigator of the damage.
            /// </summary>
            /// <param name="instigator"></param>
            /// <returns></returns>
            public Builder SetInstigator(UnitComponents instigator)
            {
                _instigator = instigator;
                return this;
            }

            /// <summary>
            /// Sets the position of the bullet that caused the damage at the time of impact.
            /// </summary>
            /// <param name="bulletPosition"></param>
            /// <returns></returns>
            public Builder SetBulletPosition(Vector3 bulletPosition)
            {
                _bulletPosition = bulletPosition;
                return this;
            }

            /// <summary>
            /// Sets the ammunition that caused the damage.
            /// </summary>
            /// <param name="damageDealer"></param>
            /// <returns></returns>
            public Builder SetDamageDealer(Ammunition damageDealer)
            {
                _damageDealer = damageDealer;
                return this;
            }

            /// <summary>
            /// Sets the type of damage (explosive or non-explosive).
            /// </summary>
            /// <param name="damageType"></param>
            /// <returns></returns>
            public Builder SetDamageType(ImpactType damageType)
            {
                _damageType = damageType;
                return this;
            }

            /// <summary>
            /// Sets whether to display a damage pop-up.
            /// </summary>
            /// <param name="displayPopUp"></param>
            /// <returns></returns>
            public Builder SetDisplayPopUp(bool displayPopUp)
            {
                _displayPopUp = displayPopUp;
                return this;
            }

            /// <summary>
            /// Builds the DamageInfo instance and returns it.
            /// </summary>
            /// <returns></returns>
            public DamageInfo Build()
            {
                return new DamageInfo(
                    _damage,
                    _direction,
                    _instigator,
                    _bulletPosition,
                    _damageDealer,
                    _damageType,
                    _displayPopUp
                );
            }
        }
    }
}