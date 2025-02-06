using System.Collections;
using System.Collections.Generic;
using TankLike.Cam;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// This class handles spawning visual effects for the unit
    /// </summary>
    public class UnitVisualEffects : MonoBehaviour, IController
    {
        [Header("Death Explosion")]
        [SerializeField] protected Vector3 _explosionSize = Vector3.one;
        [SerializeField] protected Vector3 _explosionDecalSize = Vector3.one;
        [SerializeField] protected float _explosionDecalheight = 0.1f;

        [Header("Hit Impact")]
        [SerializeField] ParticleSystem _onHitParticles;

        public bool IsActive { get; private set; }

        private TankComponents _components;

        public void SetUp(IController controller)
        {
            TankComponents components = controller as TankComponents;

            if (components == null)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;
        }

        public void PlayDeathEffects()
        {
            var explosion = GameManager.Instance.VisualEffectsManager.Explosions.DeathExplosion;
            Vector3 pos = transform.position;
            pos.y = Constants.GroundHeight;
            explosion.transform.SetPositionAndRotation(pos, Quaternion.identity);
            explosion.transform.localScale = _explosionSize;
            explosion.gameObject.SetActive(true);
            explosion.Play();
            
            var decal = GameManager.Instance.VisualEffectsManager.Explosions.ExplosionDecal;
            float randomAngle = Random.Range(0f, 360f);
            Quaternion randomRotation = Quaternion.Euler(0f, randomAngle, 0f);
            decal.transform.SetPositionAndRotation(pos, randomRotation);
            decal.transform.localScale = _explosionDecalSize;
            decal.gameObject.SetActive(true);
            decal.Play();
        }

        public void PlayOnHitEffect()
        {
            if(_onHitParticles == null)
            {
                Debug.LogError("_onHitParticles for the unit " + gameObject.name + " is null!");
                return;
            }
            _onHitParticles.Play();
        }

        #region IController
        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        public virtual void Restart()
        {
            
        }

        public virtual void Dispose()
        {
        }
        #endregion
    }
}
