using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankLike.UI;
using TMPro;
using TankLike.UnitControllers;
using TankLike.Combat.SkillTree;

namespace TankLike.UI.SkillTree
{
    public class SkillTreeInfoDisplayer : Navigatable
    {
        [Header("References")]
        [SerializeField] private SkillTreeBuilder _skillTreeBuilder;
        [SerializeField] private SkillTreeCell _centerCell;
        [SerializeField] private SkillTreeHolder _skillTreeHolder;
        //private PlayerInputActions _playerinput;

        private void Start()
        {
            AssignInputMethods();
            _skillTreeHolder.gameObject.SetActive(false);
        }

        public override void Open(int playerIndex = 0)
        {
            base.Open(playerIndex);
            _skillTreeHolder.Open();
            IsOpened = true;
            //EnableInput(true);
            _skillTreeHolder.gameObject.SetActive(true);
            GameManager.Instance.InputManager.EnableUIInput();
        }

        public override void Close(int playerIndex = 0)
        {
            if (!IsOpened) return;

            base.Close(playerIndex);
            _skillTreeHolder.Close();
            IsOpened = false;
            //EnableInput(false);
            _skillTreeHolder.gameObject.SetActive(false);
            CancelInvoke();
            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.InputManager.EnablePlayerInput();
        }

        #region Input set up
        private void AssignInputMethods()
        {
            //_playerinput.UI.Enable();
            //_playerinput.UI.Navigate_Up.performed += (context) => MoveSelection(Direction.Up);
            //_playerinput.UI.Navigate_Down.performed += (context) => MoveSelection(Direction.Down);
            //_playerinput.UI.Navigate_Right.performed += (context) => MoveSelection(Direction.Right);
            //_playerinput.UI.Navigate_Left.performed += (context) => MoveSelection(Direction.Left);
            //_playerinput.UI.Submit.performed += (context) => Select();
            //_playerinput.UI.Cancel.performed += (context) => Close(); 
        }
        #endregion

        #region Input methods
        private void MoveSelection(Direction direction)
        {
            if (!IsOpened) return;

            _skillTreeHolder.Navigate(direction);
        }
        #endregion
    }
}
