using System.Collections;
using System.Collections.Generic;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class MiniPlayerBodyParts : MonoBehaviour, IController
    {
        [SerializeField] private List<TankBodyPart> _parts;

        [Header("Death Explosion")]
        [SerializeField] protected bool _explodeOnDeath = true;
        [SerializeField] protected float _explosionForce = 10f;
        [SerializeField] protected float _explosionRadius = 2f;
        [SerializeField] protected float _upwardsModifier = 0.5f;

        protected UnitParts _bodyParts;
        private UnitComponents _components;
        private Texture2D _skinTexture;
        private Dictionary<BodyPartType, TankBodyPart> _partsDictionary = new Dictionary<BodyPartType, TankBodyPart>();

        public bool IsActive { get; set; }
        public List<TankBodyPart> Parts => _parts;

        public void SetUp(IController controller)
        {
            if (controller is not UnitComponents components)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _components = components;

            _partsDictionary.Clear();

            foreach (TankBodyPart part in _parts)
            {
                if (part is TankBody)
                {
                    _partsDictionary.Add(BodyPartType.Body, part);
                }
                else if (part is TankTurret)
                {
                    _partsDictionary.Add(BodyPartType.Turret, part);
                }
                else if (part is TankCarrier)
                {
                    _partsDictionary.Add(BodyPartType.Carrier, part);
                }
            }
        }

        public void SetTextureForMainMaterial(Texture2D texture = null)
        {
            if (texture != null)
            {
                _skinTexture = texture;
            }

            if (_skinTexture != null)
            {
                _bodyParts.SetTextureForMainMaterial(_skinTexture);
            }
        }

        public void CacheBodyParts()
        {

        }

        public TankBodyPart GetBodyPartOfType(BodyPartType type)
        {
            if (_partsDictionary.ContainsKey(type))
            {
                return _partsDictionary[type];
            }
            else
            {
                return null;
            }
        }

        public void HandlePartsExplosion()
        {
            if (!_explodeOnDeath)
            {
                return;
            }

            GameManager.Instance.SetParentToSpawnables(_bodyParts.gameObject);
            _bodyParts.gameObject.SetActive(true);

            Transform body = GetBodyPartOfType(BodyPartType.Body).transform;
            Transform turret = GetBodyPartOfType(BodyPartType.Turret).transform;

            _bodyParts.transform.position = transform.position;
            _bodyParts.transform.rotation = transform.rotation;
            _bodyParts.gameObject.SetActive(true);
            _bodyParts.StartExplosion(_explosionForce, _explosionRadius, _upwardsModifier, turret.rotation, body.rotation);

            _bodyParts = null;
        }

        #region IController
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Restart()
        {
            CacheBodyParts();
            SetTextureForMainMaterial();
        }

        public void Dispose()
        {
        }
        #endregion
    }
}
