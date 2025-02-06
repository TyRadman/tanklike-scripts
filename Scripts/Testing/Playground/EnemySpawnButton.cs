using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Testing.Playground
{
    public class EnemySpawnButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _avatarImage;

        private PlaygroundManager _playgroundManager;
        private EnemyType _enemyType;

        public void SetUp(PlaygroundManager manager, EnemyType enemyType)
        {
            _playgroundManager = manager;
            _enemyType = enemyType;
            _avatarImage.sprite = GameManager.Instance.EnemiesDatabase.GetEnemyDataFromType(enemyType).Avatar;

            _button.onClick.RemoveAllListeners();

            _button.onClick.AddListener(() =>
            {
                _playgroundManager.SpawnEnemy(enemyType);
            });
        }
    }
}
