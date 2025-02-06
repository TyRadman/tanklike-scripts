using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Combat.Tools
{
    using Utils;
    using UnitControllers;

    [CreateAssetMenu(fileName = NAME_PREFIX + "SpinningOrbs", menuName = ASSET_MENU_ROOT + "Spinning Orbs")]
    public class SpinningOrbsTool : Tool
    {
        [Header("Special Values")]
        [SerializeField] private int _orbsCount = 3;
        [SerializeField] private Bullet _orbPrefab;
        [SerializeField] private Vector2 _radiusRange;
        [SerializeField] private float _effectDuration = 3f;
        [SerializeField] private float _spinningSpeed = 5f;
        [SerializeField] private AnimationCurve _rotationSpeedCurve;
        private float _orbsMovementSpeed;

        private const float RESIZING_DURATION = 0.25f;
        private Transform _orbsParent;
        private TankComponents _playerComponents;
        private List<Bullet> _orbs = new List<Bullet>();

        public override void SetUp(TankComponents tankTransform)
        {
            base.SetUp(tankTransform);

            _playerComponents = tankTransform;
            string enemyTag = Helper.GetOpposingTag(tankTransform.gameObject.tag);

            _orbsParent = new GameObject($"Orbs {_orbPrefab.name}").transform;
            _orbsParent.position = tankTransform.transform.position;

            float angleIncrement = 360 / _orbsCount;

            // create the orb instances
            for (int i = 0; i < _orbsCount; i++)
            {
                Bullet orb = Instantiate(_orbPrefab, _orbsParent);
                // turn off the bullet
                orb.OnRelease();
                // set reference to the bullet data
                orb.SetUpBulletdata(GameManager.Instance.BulletsDatabase.GetBulletDataFromGUID("BD_Bullet_01"));
                // set enemy tags as hittables
                orb.SetUp(_playerComponents);
                // rotate the orb out to make it move forward when the tool is activated
                orb.transform.eulerAngles = Vector3.up * (i * angleIncrement);
                // put the orb at the starting point
                orb.transform.localPosition = Vector3.zero;
                orb.transform.localPosition = orb.transform.forward * _radiusRange.x;
                // add it to the list
                _orbs.Add(orb);
            }

            _orbsMovementSpeed = (_radiusRange.y - _radiusRange.x) / _effectDuration;
        }

        public override void SetDuration()
        {
            _duration = RESIZING_DURATION * 2 + _effectDuration;
            _cooldownDuration += _duration;
        }

        public override void UseTool()
        {
            base.UseTool();

            _orbsParent.position = _playerComponents.transform.position;
            _orbs.ForEach(o => o.EnableBullet(true));
            _tank.StartCoroutine(SpinningProcess());
        }

        private IEnumerator SpinningProcess()
        {
            float time = 0f;
            Vector3 rotation = Vector3.up * _spinningSpeed;
            Transform playerTank = _playerComponents.transform;

            // scale up the orbs
            while (time < RESIZING_DURATION)
            {
                time += Time.deltaTime;
                float t = time / RESIZING_DURATION;
                // make the orbs' parent follow the tank
                _orbsParent.position = playerTank.position;
                // scale the orbs
                _orbs.ForEach(o => o.transform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, t));
                yield return null;
            }

            time = 0f;

            // spin the orbs
            while (time < _effectDuration)
            {
                time += Time.deltaTime;
                float t = time / _effectDuration;

                // make the orbs' parent follow the tank
                _orbsParent.position = playerTank.position;

                // spin the orbs
                _orbsParent.Rotate(_rotationSpeedCurve.Evaluate(t) * Time.deltaTime * rotation);

                // move the orbs outwards
                _orbs.ForEach(o => o.MoveBulletForFrame(Vector3.zero, _orbsMovementSpeed));

                yield return null;
            }

            time = 0f;

            // scale down the orbs
            while (time < RESIZING_DURATION)
            {
                time += Time.deltaTime;
                float t = time / RESIZING_DURATION;
                _orbs.ForEach(o => o.transform.localScale = Vector3.one * Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            // disable and reset the orbs
            foreach (Bullet orb in _orbs)
            {
                // disable the bullet
                orb.EnableBullet(false);
                // put the orb at the starting point
                orb.transform.localPosition = Vector3.zero;
                orb.transform.position = orb.transform.position + orb.transform.forward * _radiusRange.x;
            }
        }

        public override void ResetValues(Transform tankTransform)
        {
            base.ResetValues(tankTransform);
        }

        public override void Dispose()
        {
            base.Dispose();

            // destroy the bullets and their parent
            Destroy(_orbsParent.gameObject);
        }

    }
}
