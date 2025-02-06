using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.Utils;
    using UnitControllers;

    public class EnemiesCombatManager : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }

        private List<EnemyComponents> _currentEnemies = new List<EnemyComponents>();
        private int _currentAttackingEnemyIndex = 0;
        private int _allowAttacksCount = 2;
        private int _currentAttackCount = 0;
        private bool _isAttackingAllowed = false;
        private float _attackCooldown = 0.5f;

        private readonly int _numberOfEnemiesPerAttacker = 3;


        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void AddEnemyWave(List<EnemyComponents> enemies)
        {
            _currentEnemies.Clear();
            _currentEnemies.AddRange(enemies);

            _currentAttackingEnemyIndex = 0;

            _allowAttacksCount = Mathf.CeilToInt((float)_currentEnemies.Count / (float)_numberOfEnemiesPerAttacker);
            _currentAttackCount = 0;
            _isAttackingAllowed = true;
        }

        public bool RequestAttack(EnemyComponents enemy)
        {
            if(!_currentEnemies.Contains(enemy))
            {
                string debug = string.Empty;

                foreach (var e in _currentEnemies)
                {
                    debug += e.name + ", ";
                }

                Debug.LogError($"Enemy {enemy.name} not found in current enemies list. Enemies: {debug}");

                //Debug.LogError($"Enemy {enemy.name} not found in current enemies list.");
                return false;
            }

            if (_currentEnemies[_currentAttackingEnemyIndex] != enemy || !_isAttackingAllowed)
            {
                Debug.Log($"Enemy {enemy.name} requested an attack and was denied".Color(Colors.Red));
                return false;
            }

            _currentAttackingEnemyIndex++;

            if(_currentAttackingEnemyIndex >= _currentEnemies.Count)
            {
                _currentAttackingEnemyIndex = 0;
            }

            _currentAttackCount++;

            if(_currentAttackCount >= _allowAttacksCount)
            {
                _currentAttackCount = 0;
                _isAttackingAllowed = false;
                Debug.Log($"Enemy {enemy.name} requested an attack and was granted".Color(Colors.Green));
                StartCoroutine(CooldownRoutine());
            }

            return true;
        }

        // TODO: add removving enemies when they die

        private IEnumerator CooldownRoutine()
        {
            float timeElapsed = 0f;

            Debug.Log($"Cooldown started".Color(Colors.LightOrange));

            while (timeElapsed < _attackCooldown)
            {
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            Debug.Log($"Cooldown ended".Color(Colors.DarkOrange));
            _isAttackingAllowed = true;
        }
    }
}
