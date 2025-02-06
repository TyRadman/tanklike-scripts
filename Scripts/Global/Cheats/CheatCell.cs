using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    public class CheatCell : ScriptableObject
    {
        public string CheatText;
        [HideInInspector] public CheatButton Button;
        [HideInInspector] protected bool _enabled = false;

        public virtual void PerformCheat()
        {

        }

        public virtual void Initiate()
        {
            _enabled = false;
        }
    }
}
