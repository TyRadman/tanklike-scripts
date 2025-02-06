using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.Testing.Playground
{
    public class PlaygroundUIController : MonoBehaviour
    {
        [Header("Enemy Spawn")]
        [SerializeField] private Transform _enemySpawnButtonsParent;
        [SerializeField] private EnemySpawnButton _enemySpawnButtonPrefab;

        [Header("Activation Button")]
        [SerializeField] private Button _activationButton;
        [SerializeField] private Image _activationButtonImage;
        [SerializeField] private TextMeshProUGUI _activationButtonText;
        [SerializeField] private Color _activatedColor;
        [SerializeField] private Color _deactivatedColor;

        [Header("Animations")]
        [SerializeField] private Animation _enemySelectionPanelAnimation;
        [SerializeField] private AnimationClip _panelOpenClip;
        [SerializeField] private AnimationClip _panelCloseClip;

        private const string ACTIVATION_TEXT = "Activate";
        private const string DEACTIVATION_TEXT = "Deactivate";
        
        private List<EnemySpawnButton> _enemySpawnButtons = new List<EnemySpawnButton>();
        private bool _enemiesAreActivated;

        public void SetUp(PlaygroundManager playgroundManager)
        {
            //gameObject.SetActive(false);

            GameManager.Instance.EnemiesDatabase.GetAllEnemies().ForEach(e =>
            {
                EnemySpawnButton button = Instantiate(_enemySpawnButtonPrefab, _enemySpawnButtonsParent);
                button.SetUp(playgroundManager, e.EnemyType);
                _enemySpawnButtons.Add(button);
            });

            _activationButton.onClick.RemoveAllListeners();
            _activationButton.onClick.AddListener(HandleEnemiesActivation);

            // Set up activation button
            _enemiesAreActivated = false;
            HandleEnemiesActivation();
        }

        public void Open()
        {
            _enemySelectionPanelAnimation.clip = _panelOpenClip;
            _enemySelectionPanelAnimation.Play();
            //gameObject.SetActive(true);
        }

        public void Close()
        {
            _enemySelectionPanelAnimation.clip = _panelCloseClip;
            _enemySelectionPanelAnimation.Play();
            //gameObject.SetActive(false);
        }

        private void HandleEnemiesActivation()
        {
            _enemiesAreActivated = !_enemiesAreActivated;

            if (_enemiesAreActivated)
            {
                GameManager.Instance.EnemiesManager.ActivateSpawnedEnemies();
                _activationButtonImage.color = _deactivatedColor;
                _activationButtonText.text = DEACTIVATION_TEXT;
            }
            else
            {
                GameManager.Instance.EnemiesManager.DeactivateSpawnedEnemies();
                _activationButtonImage.color = _activatedColor;
                _activationButtonText.text = ACTIVATION_TEXT;
            }
        }
    }
}
