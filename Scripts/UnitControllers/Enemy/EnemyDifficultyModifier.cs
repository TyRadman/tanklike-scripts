using System.Collections;
using System.Collections.Generic;
using TankLike.Utils;
using UnityEngine;

namespace TankLike.UnitControllers
{
    public class EnemyDifficultyModifier : MonoBehaviour, IController
    {
        [SerializeField] private List<DifficultyModifier> _modifiers;

        public bool IsActive { get; set; }
        
        private EnemyComponents _enemyComponents;

        public void SetUp(IController controller)
        {
            if (controller is not EnemyComponents enemyComponents)
            {
                Helper.LogWrongComponentsType(GetType());
                return;
            }

            _enemyComponents = enemyComponents;
        }

        public void ApplyModifier(DifficultyModifier modifier, float difficulty = 0f)
        {
            modifier.ApplyModifier(_enemyComponents, difficulty);
        }

        public T GetModifier<T>() where T : DifficultyModifier
        {
            T modifier = _modifiers.Find(m => m is T) as T;

            if(modifier == null)
            {
                Debug.LogError($"Modifier {typeof(T)} not found in {name}");
            }

            return modifier;
        }

        #region IController
        public void Activate()
        {

        }

        public void Deactivate()
        {

        }

        public void Restart()
        {
            float difficulty = GameManager.Instance.EnemiesManager.Difficulty;
            _modifiers.ForEach(m => m.ApplyModifier(_enemyComponents, difficulty));
        }

        public void Dispose()
        {

        }
        #endregion
    }
}
