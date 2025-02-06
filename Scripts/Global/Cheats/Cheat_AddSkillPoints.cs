using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "AddSkillPoints", menuName = ROOT + "Add Skill Points")]
    public class Cheat_AddSkillPoints : Cheat
    {
        private const int SKILL_POINTS_TO_ADD = 10;

        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            GameManager.Instance.PlayersManager.GetPlayerProfiles().ForEach(p => p.Upgrades.AddSkillPoints(SKILL_POINTS_TO_ADD));
        }
    }
}
