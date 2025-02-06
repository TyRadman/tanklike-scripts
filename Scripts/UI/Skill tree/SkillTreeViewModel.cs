using System;
using System.Collections;
using System.Collections.Generic;
using TankLike.Combat.SkillTree;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    public class SkillTreeViewModel
    {
        private PlayerComponents _player;
        private SkillTreeUIDisplayer _UIDisplayer;

        public SkillTreeViewModel(PlayerComponents player, SkillTreeUIDisplayer UIDisplayer)
        {
            _player = player;
            _UIDisplayer = UIDisplayer;
        }

        public void UpdatePlayerSkillPointsCount()
        {
            int playerPoints = _player.Upgrades.GetSkillPoints();
            _UIDisplayer.UpdatePlayerSkillPointsCount(playerPoints);
        }

        internal void Update()
        {
            UpdatePlayerSkillPointsCount();
        }
    }
}
