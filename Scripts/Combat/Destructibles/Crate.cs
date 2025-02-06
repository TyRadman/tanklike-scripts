using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Destructible
{
    using UnitControllers;
    using Utils;

    public class Crate : DestructibleDropper
    {
        [Header("Special Values")]
        [SerializeField] private float _explosionForce = 30f;
        [SerializeField] private float _explosionRadius = 2f;

        [Header("References")]
        [SerializeField] private List<Rigidbody> _parts;
        [SerializeField] private GameObject _partsParent;
        [SerializeField] private GameObject _GFX;
        [SerializeField] private BoxCollider _collider;
        
        [Header("Animations")]
        [SerializeField] private Animation _anim;

        protected override void OnDestructibleDeath(UnitComponents tank)
        {
            base.OnDestructibleDeath(tank);

            // report the destruction of the object to the report manager
            int playerIndex = -1;

            if (tank is PlayerComponents components)
            {
                playerIndex = components.PlayerIndex;
            }

            GameManager.Instance.ReportManager.ReportDestroyingObject(DropperTag, playerIndex);

            CollectablesDropRequest collectablesDropSettings = new CollectablesDropRequest()
            {
                Position = transform.position,
                DropperTag = DropperTag,
                Settings = _dropSettings,
                DropPassedTags = true,
                Drops = _drops
            };

            GameManager.Instance.CollectableManager.SpawnCollectablesOfType(collectablesDropSettings);

            StartCoroutine(ExplosionProcess());
        }

        private IEnumerator ExplosionProcess()
        {
            _partsParent.SetActive(true);
            _GFX.SetActive(false);
            _collider.enabled = false;

            yield return null;

            for (int i = 0; i < _parts.Count; i++)
            {
                _parts[i].isKinematic = false;
                _parts[i].useGravity = true;
            }

            yield return null;

            CrateExplode();

            yield return new WaitForSeconds(3f);

            this.PlayAnimation(_anim);
        }

        public void CrateExplode()
        {
            Vector3 randomOffset = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            _parts.ForEach(p => p.AddExplosionForce(_explosionForce, transform.position + randomOffset, _explosionRadius));
        }
    }
}