using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public abstract class DifficultyModifier : ScriptableObject
    {
        public const string MENU_NAME = "TankLike/Enemies/Difficulty Modifiers/";

        public virtual void ApplyModifier(TankComponents enemy, float difficulty)
        {

        }
    }
}
