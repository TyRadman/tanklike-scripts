using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class TankCarrier : TankBodyPart
    {
        [field: SerializeField] public Animator Animator { private set; get; }
        [field: SerializeField] public List<ParticleSystem> DustParticles { private set; get; }
        [field: SerializeField] public List<ParticleSystem> TracksParticles { private set; get; }
        [field: SerializeField] public List<ParticleSystem> ThrustersParticles { private set; get; }
    }
}
