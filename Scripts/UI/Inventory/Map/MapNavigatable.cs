using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.UI.Inventory
{
    using Signifiers;
    using UI.Map;
    using Utils;
    using Environment;

    public class MapNavigatable : Navigatable, IInput
    {
        [Header("References")]
        [SerializeField] private RectTransform _mapBody;
        [Header("Movement")]
        [SerializeField] private float _movementSpeed;
        [SerializeField] private LevelMapDisplayer _mapDisplayer;
        
        private Vector2 _movementDirection;
        private Coroutine _movementCoroutine;

        private const float MAP_LIMIT_UP = 400f;
        private const float MAP_LIMIT_DOWN = -400f;
        private const float MAP_LIMIT_LEFT = -400f;
        private const float MAP_LIMIT_RIGHT = 400f;

        private void Start()
        {
            ClampCameraPosition();
        }

        #region Open and Close
        public override void Open(int playerIndex)
        {
            base.Open(PlayerIndex);
            SetPlayerIndex(playerIndex);
            SetUpInput(playerIndex);
            _mapDisplayer.OnMapOpened();
            _movementDirection = Vector2.zero;


            this.StopCoroutineSafe(_movementCoroutine);

            _movementCoroutine = StartCoroutine(MovementProcess());
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);
            DisposeInput(playerIndex);
            SetPlayerIndex(-1);

            this.StopCoroutineSafe(_movementCoroutine);
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed += NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed += NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;
            UIMap.FindAction(c.UI.Navigate_Left.name).canceled += CancelNavigationLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).canceled += CancelNavigationRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).canceled += CancelNavigationUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled += CancelNavigationDown;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Navigate_Left.name).performed -= NavigateLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).performed -= NavigateRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;
            UIMap.FindAction(c.UI.Navigate_Left.name).canceled -= CancelNavigationLeft;
            UIMap.FindAction(c.UI.Navigate_Right.name).canceled -= CancelNavigationRight;
            UIMap.FindAction(c.UI.Navigate_Up.name).canceled -= CancelNavigationUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled -= CancelNavigationDown;
        }

        #region Input Methods
        public override void NavigateLeft(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Left);
        }
        public override void NavigateRight(InputAction.CallbackContext _)
        {
            base.NavigateRight(_);
            Navigate(Direction.Right);
        }
        public override void NavigateUp(InputAction.CallbackContext _)
        {
            base.NavigateUp(_);
            Navigate(Direction.Up);
        }
        public override void NavigateDown(InputAction.CallbackContext _)
        {
            base.NavigateLeft(_);
            Navigate(Direction.Down);
        }
        public void CancelNavigationLeft(InputAction.CallbackContext _)
        {
            StopNavigation(Direction.Left);
        }
        public void CancelNavigationRight(InputAction.CallbackContext _)
        {
            StopNavigation(Direction.Right);
        }
        public void CancelNavigationUp(InputAction.CallbackContext _)
        {
            StopNavigation(Direction.Up);
        }
        public void CancelNavigationDown(InputAction.CallbackContext _)
        {
            StopNavigation(Direction.Down);
        }
        #endregion
        #endregion

        public override void DehighlightCells()
        {
            base.DehighlightCells();
            StopAllCoroutines();
        }

        private IEnumerator MovementProcess()
        {
            while (IsOpened)
            {
                _mapBody.localPosition += new Vector3(_movementDirection.x, _movementDirection.y, 0f) * Time.unscaledDeltaTime;
                ClampCameraPosition();
                yield return null;
            }
        }

        private void ClampCameraPosition()
        {
            float x = Mathf.Clamp(_mapBody.localPosition.x, MAP_LIMIT_LEFT, MAP_LIMIT_RIGHT);
            float y = Mathf.Clamp(_mapBody.localPosition.y, MAP_LIMIT_DOWN, MAP_LIMIT_UP);
            _mapBody.localPosition = new Vector3(x, y, 0);
        }

        public override void Navigate(Direction direction)
        {
            if (!IsOpened)
            {
                return;
            }

            base.Navigate(direction);

            ChangeMovementDirection(direction, 1f);
        }

        private void StopNavigation(Direction direction)
        {
            if (!IsOpened)
            {
                return;
            }

            ChangeMovementDirection(direction, -1f);
        }

        private void ChangeMovementDirection(Direction direction, float multiplier)
        {
            switch (direction)
            {
                case Direction.Up:
                    {
                        _movementDirection.y += -_movementSpeed * multiplier;
                        break;
                    }
                case Direction.Down:
                    {
                        _movementDirection.y += _movementSpeed * multiplier;
                        break;
                    }
                case Direction.Left:
                    {
                        _movementDirection.x += _movementSpeed * multiplier;
                        break;
                    }
                case Direction.Right:
                    {
                        _movementDirection.x += -_movementSpeed * multiplier;
                        break;
                    }
            }
        }
    }
}
