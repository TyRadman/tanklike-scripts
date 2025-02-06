using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using Cheats;
    using UnitControllers;

    [CreateAssetMenu(fileName = NAME + "Constraint", menuName = ROOT + "Constraint")]
    public class Cheat_Constraint : Cheat
    {
        [SerializeField] private AbilityConstraint _constaint;

        public override void Initiate()
        {
            base.Initiate();
            Button.SetButtonColor(false);
        }

        public override void PerformCheat()
        {
            _enabled = !_enabled;
            GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Constraints.ApplyConstraints(_enabled, _constaint));
            Button.SetButtonColor(_enabled);
        }
    }
}
