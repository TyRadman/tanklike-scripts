using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Tutorial
{

    [CreateAssetMenu(fileName = FILE_NAME_PREFIX + "HealthCharge", menuName = MENU_PATH + "Health Charge")]
    public class HealthChargeAchievement : TutorialAchievement
    {
        private bool _isHealthFullyCharged = false;

        public override void SetUp(WaypointMarker marker)
        {
            base.SetUp(marker); 
            
            _isHealthFullyCharged = false;

            _waypointMarker.Manager.GetPlayer().Health.OnHealthFullyCharged += MarkAcievementAsCompleted;
        }

        private void MarkAcievementAsCompleted()
        {
            _isHealthFullyCharged = true;

            if (IsAchieved())
            {
                OnAchievementCompleted?.Invoke();
                _waypointMarker.Manager.GetPlayer().Health.OnHealthFullyCharged += MarkAcievementAsCompleted;
            }
        }

        public override bool IsAchieved()
        {
            return _isHealthFullyCharged;
        }
    }
}
