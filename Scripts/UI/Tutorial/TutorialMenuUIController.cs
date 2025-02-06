using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI
{
    public class TutorialMenuUIController : MonoBehaviour
    {
        [System.Serializable]
        public struct TutorialTab
        {
            public Button TabButton;
            public GameObject Content;
            public Image Background;
        }

        [SerializeField] private GameObject _tutorialParent;
        [SerializeField] private GameObject _lobbyParent;
        [SerializeField] private List<TutorialTab> _tutorialTabs = new List<TutorialTab>();

        [Header("Colors")]
        [SerializeField] private Color _buttonNormalColor;
        [SerializeField] private Color _buttonHighlightColor;

        private bool _tutorialIsActive = false;

        private void Awake()
        {
            foreach (var tab in _tutorialTabs)
            {
                tab.TabButton.onClick.AddListener(() =>
                {
                    DisplayTab(tab);
                });
            }

            _tutorialParent.SetActive(false);
            DisplayTab(_tutorialTabs[0]);
        }

        private void DisplayTab(TutorialTab tab)
        {
            _tutorialTabs.ForEach(t => {
                t.Content.SetActive(false);
                t.Background.color = _buttonNormalColor;
                });

            tab.Content.SetActive(true);
            tab.Background.color = _buttonHighlightColor;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (!_tutorialIsActive)
                {
                    DisplayTab(_tutorialTabs[0]);
                }

                _tutorialIsActive = !_tutorialIsActive;
                _tutorialParent.SetActive(_tutorialIsActive);
                _lobbyParent.SetActive(!_tutorialIsActive);
                Cursor.visible = _tutorialIsActive;
                Cursor.lockState = _tutorialIsActive ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        public void Dispose()
        {
            foreach (var tab in _tutorialTabs)
            {
                tab.TabButton.onClick.RemoveAllListeners();
            }
        }
    }
}
