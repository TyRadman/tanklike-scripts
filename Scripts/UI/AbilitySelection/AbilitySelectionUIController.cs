using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class AbilitySelectionUIController : MonoBehaviour
    {
        [SerializeField] protected AbilitySelectionData _selectionData;

        [SerializeField] private AbilitySelectionPanel _normalShotParent;
        [SerializeField] private AbilitySelectionPanel _holdAbilityParent;
        [SerializeField] private AbilitySelectionPanel _superAbilityParent;
        [SerializeField] private AbilitySelectionPanel _boostAbilityParent;

        [Header("Loading Bar")]
        [SerializeField] private GameObject _loadingBar;
        [SerializeField] private Image _loadingBarImage;

        private AbilitySelectionPanel _currentSelectedPanel;

        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _loadingBar.SetActive(false);

            _normalShotParent.Setup();
            _holdAbilityParent.Setup();
            _superAbilityParent.Setup();
            _boostAbilityParent.Setup();

            _currentSelectedPanel = _normalShotParent;
            DisplayNextAbility(_normalShotParent);
        }

        public void DisplayNextAbility(AbilitySelectionPanel panel)
        {
            _currentSelectedPanel.gameObject.SetActive(false);
            _currentSelectedPanel = panel;
            _currentSelectedPanel.gameObject.SetActive(true);
        }

        public void LoadLobbyScene()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            GameManager.Instance.DisposeCurrentSceneController();
            GameManager.Instance.SceneLoadingManager.SwitchScene(Scenes.ABILITY_SELECTION, Scenes.LOBBY);
        }
    }
}
