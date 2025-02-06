using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace TankLike.Cheats
{
    public class CheatsManager : MonoBehaviour
    {
        [SerializeField] private Button _returnButton;
        [SerializeField] private List<CheatButton> _cheatButtons = new List<CheatButton>();
        [SerializeField] private CheatsParent _mainCheats;
        [SerializeField] private bool _cheatsActive = false;
        [SerializeField] private GameObject _cheatsVisuals;
        private CheatsParent _currentParent;

        private void Awake()
        {
            _returnButton.onClick.AddListener(OnReturnButtonClicked);
            _cheatsVisuals.SetActive(false);
            _cheatButtons.ForEach(b => b.SetUp(this));
            BindButtonsToCheats(_mainCheats);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                _cheatsActive = !_cheatsActive;
                _cheatsVisuals.SetActive(_cheatsActive);
                Cursor.visible = _cheatsActive;
                Cursor.lockState = _cheatsActive ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }

        public void BindButtonsToCheats(CheatsParent parent)
        {
            parent.PreviousParent = _currentParent;
            _currentParent = parent;
            List<CheatCell> cheats = parent.Cheats;

            if(cheats.Count > _cheatButtons.Count)
            {
                Debug.LogError("There are more cheats than there are buttons");
                return;
            }

            for (int i = 0; i < cheats.Count; i++)
            {
                _cheatButtons[i].gameObject.SetActive(true);
                _cheatButtons[i].AssignCheat(cheats[i]);
                cheats[i].Initiate();
            }

            for (int i = _cheatButtons.Count - 1; i >= cheats.Count; i--)
            {
                _cheatButtons[i].gameObject.SetActive(false);
            }
        }
        private void OnReturnButtonClicked()
        {
            if (_currentParent.PreviousParent)
            {
                BindButtonsToCheats(_currentParent.PreviousParent);
            }
        }
    }
}
