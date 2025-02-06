using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Tutorial
{
    public abstract class TutorialAchievement : ScriptableObject
    {
        public const string MENU_PATH = Directories.MAIN + "Tutorial/Achievements/";
        public const string FILE_NAME_PREFIX = "Achievement_";
        public abstract bool IsAchieved();
        public System.Action OnAchievementCompleted { get; set; }
        protected TutorialManager _tutorialManager;
        protected WaypointMarker _waypointMarker;

        public virtual void SetUp(WaypointMarker marker)
        {
            _waypointMarker = marker;
            _tutorialManager = marker.Manager;
        }
    }
}
