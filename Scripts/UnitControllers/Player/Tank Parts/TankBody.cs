using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Combat;

namespace TankLike.UnitControllers
{
    public class TankBody : TankBodyPart
    {
        [field: SerializeField] public CollisionEventPublisher Bumper { private set; get; }
        [field: SerializeField] public List<ParticleSystem> BoostParticles { private set; get; }
        [field: SerializeField] public ParticleSystem LandParticles { private set; get; }
        [field: SerializeField] public List<DamageDetector> DamageDetectors { private set; get; }

        public void Setup(string tag)
        {
            gameObject.tag = tag;
        }
    }
}
