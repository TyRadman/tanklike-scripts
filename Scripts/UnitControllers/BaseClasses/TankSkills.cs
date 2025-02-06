using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using UnityEngine;

namespace TankLike.UnitControllers
{
    // TODO: delete this if not needed
    public abstract class TankSkills : MonoBehaviour
    {
        private List<Skill> _relics = new List<Skill>();

        public virtual void AddRelic(Skill relic)
        {
            // temporary. We need to look into whether we need scriptable objects for this, or stick to in-game instances
            //Skill newRelic = Instantiate(relic, transform);
            //_relics.Add(newRelic);
            //newRelic.SetUp(transform);
        }
    }
}
