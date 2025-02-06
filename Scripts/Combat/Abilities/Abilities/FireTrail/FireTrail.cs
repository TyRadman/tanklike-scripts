using UnityEngine;

namespace TankLike.Combat.Abilities
{
    using Utils;
    using UnitControllers;
    using Combat.SkillTree;

	[CreateAssetMenu(fileName = PREFIX + "FireTrail", menuName = Directories.ABILITIES + "Fire Trail")]
	public class FireTrail : Ability
	{
		[Header("Special Values")]
		[SerializeField] private FireTrailSpot _fireTrailPrefab;
		[SerializeField] private int _damage = 10;
		[SerializeField] private float _fireSpotDuration = 3f;
		[SerializeField] private GameTags _tagetTag;

        private Pool<FireTrailSpot> _fireTrailSpotPool;

        private const int POOL_PREFILL_COUNT = 5;

        public override void SetUp(TankComponents components)
        {
            base.SetUp(components);
            _fireTrailSpotPool = CreatePool(_fireTrailPrefab);
        }

        public override void PopulateSkillProperties()
        {
            base.PopulateSkillProperties();

            AddSkillProperty("Fire damage per second", _fireTrailPrefab.GetDamage(), Colors.DarkOrange, PropertyUnits.POINTS);
        }

        public override void PerformAbility()
		{
			Vector3 position = _components.transform.position;
			position.y = Constants.GroundHeight;

            FireTrailSpot fireSpot = _fireTrailSpotPool.RequestObject(position, Quaternion.identity);
            fireSpot.gameObject.SetActive(true);

			fireSpot.SetUp(_damage, _tagetTag.ToString(), _fireSpotDuration);
		}

        private Pool<FireTrailSpot> CreatePool(FireTrailSpot prefab)
        {
            var pool = new Pool<FireTrailSpot>(
                () =>
                {
                    var obj = MonoBehaviour.Instantiate(prefab);
                    GameManager.Instance.SetParentToRoomSpawnables(obj.gameObject);
                    return obj;
                },
                (FireTrailSpot obj) => obj.GetComponent<IPoolable>().OnRequest(),
                (FireTrailSpot obj) => obj.GetComponent<IPoolable>().OnRelease(),
                (FireTrailSpot obj) => obj.GetComponent<IPoolable>().Clear(),
                POOL_PREFILL_COUNT
           );
            return pool;
        }

        public override void OnAbilityHoldStart()
        {

        }

        public override void OnAbilityHoldUpdate()
        {

        }

        public override void OnAbilityFinished()
        {

        }

        public override void OnAbilityInterrupted()
        {

        }

        public override void SetUpIndicatorSpecialValues(AirIndicator indicator)
        {

        }

        public override void Dispose()
        {

        }
    }
}