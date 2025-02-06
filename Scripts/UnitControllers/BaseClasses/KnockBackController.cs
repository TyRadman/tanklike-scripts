using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    using Utils;

    public class KnockBackController : MonoBehaviour, IController
    {
        public bool IsActive { get; private set; }

        private TankWiggler _wiggler;
        private TankHealth _health;
        [SerializeField] private Wiggle _onHitWiggle;

        public void SetUp(IController controller)
        {
            if(controller is not TankComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _wiggler = components.TankWiggler;
            _health = components.Health;
        }

        public void OnHit()
        {
            _wiggler.WiggleBody(_onHitWiggle);
        }

        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Restart()
        {
            _health.OnHit += OnHit;
        }

        public void Dispose()
        {
            _health.OnHit = null;
        }
    }
}
