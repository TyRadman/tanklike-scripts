using System.Collections;
using System.Collections.Generic;
using TankLike.ItemsSystem;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class UnitParts : MonoBehaviour, IPoolable
    {
        [SerializeField] private Transform _turretPartsParent;
        [SerializeField] private Transform _bodyPartsParent;
        [SerializeField] private List<TankPart> _parts;
        [SerializeField] private List<CoinCollectable> _collectableParts;
        public System.Action<IPoolable> OnReleaseToPool { get; private set; }

        [SerializeField] private float _collectablesScatterRadius = 1f;

        public void SetTextureForMainMaterial(Texture2D texture)
        {
            Material material = _parts[0].Renderer.material;
            material.SetTexture("_BaseMap", texture);
            _parts.ForEach(p => p.Renderer.material = material);
        }

        public void StartExplosion(float force, float radius, float upwardsModifier, Quaternion turretRotation, Quaternion bodyRotation, bool bossPreShrink = false)
        {
            _turretPartsParent.rotation = turretRotation;
            _bodyPartsParent.rotation = bodyRotation;
            StartCoroutine(ExplosionProcess(force, radius, upwardsModifier, bossPreShrink));
        }

        private IEnumerator ExplosionProcess(float force, float radius, float upwardsModifier, bool bossPreShrink = false)
        {
            yield return null;

            for (int i = 0; i < _parts.Count; i++)
            {
                _parts[i].RigidBody.isKinematic = false;
                _parts[i].RigidBody.useGravity = true;
            }

            Vector3 randomOffset = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            _parts.ForEach(p => p.RigidBody.AddExplosionForce(force, transform.position + randomOffset, radius, upwardsModifier, ForceMode.Impulse));
            // assign shrinking time for each part
            _parts.ForEach(p => p.StartShrinkingCountDown(GameManager.Instance.Constants.Collectables.GetPreShrinkTime(bossPreShrink)));

            Vector3 startPosition = transform.position;

            // randomize the position of the collectable parts
            for (int i = 0; i < _collectableParts.Count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * _collectablesScatterRadius;
                //_collectableParts[i].transform.parent = null;
                _collectableParts[i].transform.position = startPosition + new Vector3(offset.x, 0f, offset.y);
                _collectableParts[i].StartCollectable();
            }

            ConstantsManager constants = GameManager.Instance.Constants;
            float lifetime = constants.Collectables.BounceTime + constants.Collectables.GetPreShrinkTime(bossPreShrink) + constants.Collectables.DisplayDuration;
            yield return new WaitForSeconds(lifetime);

            TurnOff();
        }

        #region Pool
        public void Init(System.Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void TurnOff()
        {
            OnReleaseToPool(this);
        }

        public void OnRequest()
        {
        }

        public void OnRelease()
        {
            gameObject.SetActive(false);

            _parts.ForEach(p =>
            {
                p.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero)); 
                p.RigidBody.isKinematic = true;
                p.RigidBody.useGravity = false;
                p.gameObject.SetActive(true);
                p.transform.localScale = Vector3.one;
            });

            _collectableParts.ForEach(c =>
            {
                c.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
                c.gameObject.SetActive(true);
                c.transform.localScale = Vector3.one;
            });

            GameManager.Instance.SetParentToSpawnables(gameObject);
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
        #endregion
    }
}
