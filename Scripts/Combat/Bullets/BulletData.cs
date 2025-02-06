using System.Collections;
using System.Collections.Generic;
using TankLike.Elements;
using TankLike.Sound;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    public class BulletData
    {
        public float Speed;
        public float MaxDistance;
        public int Damage;
        public bool CanBeDeflected = true;
        public bool UseGravity;
        public float GravityMultiplier;

        public Deflection Deflection;
        public SpeedOverLife SpeedOverLife;
        public OnImpact Impact;
        public ElementEffect Element;

        public Audio OnShotAudio;

        public void SetAOERadius(float radius)
        {
            if(Impact is AreaOfEffectImpact aoeImpact)
            {
                aoeImpact.SetAreaRadius(radius);
            }
        }
    }
}
