using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.Combat
{
    public class TankHealthEffect : MonoBehaviour
    {
        [System.Serializable]
        private class ParticleSettings
        {
            public ParticleSystem Particles;
            public Vector2 ParticleCountRange;
            public AnimationCurve ParticleStartCurve;
        }

        [SerializeField] private List<ParticleSettings> _particles;

        public void SetIntensity(float intensty)
        {
            intensty = 1 - intensty;

            foreach (ParticleSettings particleSettings in _particles)
            {
                ParticleSystem particles = particleSettings.Particles;
                float t = particleSettings.ParticleStartCurve.Evaluate(intensty);

                if (t == 0)
                {
                    particles.Stop();
                    continue;
                }

                var emission = particles.emission;
                emission.rateOverTime = particleSettings.ParticleCountRange.Lerp(t);

                particles.Play();
            }
        }
    }
}
