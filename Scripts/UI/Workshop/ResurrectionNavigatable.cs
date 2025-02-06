using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Workshop
{
    using Signifiers;
    using Environment;
    using TankLike.UnitControllers;
    using TankLike.Utils;

    public class ResurrectionNavigatable : Navigatable, IInput
    {
        [SerializeField] private float _loadDuration = 3f;
        [SerializeField] private Color _sufficientCoinsColor;
        [SerializeField] private Color _insufficientCoinsColor;

        [Header("References")]
        [SerializeField] private Image _processBar;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _resurrectionActionText;
        [SerializeField] private TextMeshProUGUI _coinsWarningText;
        [SerializeField] private TextMeshProUGUI _detailsText;

        private const float WARNING_DURATION = 1.5f;
        private const string REVIVE_TEXT = "Revive player ";
        private const string REVIVE_ACTION_TEXT = " - Hold ";
        private const string NO_REVIVAL_MESSAGE = "No dead players to revive";
        private const string ACTION_TEXT = "Revive";
        private string _actionKey;
        private bool _revived = false;
        private UIActionSignifiersController _actionSignifiersController;
        private Workshop_InteractableArea _workshopInteractableArea;

        private void Start()
        {
            _coinsWarningText.enabled = false;
        }

        public override void SetUp()
        {
            base.SetUp();

            _workshopInteractableArea = GameManager.Instance.WorkshopController.WorkShopArea;

            if(_workshopInteractableArea == null)
            {
                Debug.LogError("Workshop interactable area is not set.");
            }
        }

        #region Open and Close
        public override void Open(int playerIndex)
        {
            if (!IsPlayerDead())
            {
                _detailsText.enabled = false;
                // do other things to indicate the page is not available like changing the font color
                _resurrectionActionText.text = NO_REVIVAL_MESSAGE;
                return;
            }

            _detailsText.enabled = true;
            // set the text that will display the message to the player "Revive player 2 - hold Q" for instance
            SetReviveText();
            base.Open(playerIndex);
            SetPlayerIndex(playerIndex);
            SetUpInput(playerIndex);
            
            _coinsText.text = $"{Constants.REVIVAL_COST} / {GameManager.Instance.PlayersManager.Coins.CoinsAmount}";
            _coinsText.color = PlayerHasEnoughCoins() ? _sufficientCoinsColor : _insufficientCoinsColor;
        }

        public override void Close(int playerIndex)
        {
            if (!IsPlayerDead())
            {
                return;
            }

            base.Close(playerIndex);
            SetPlayerIndex(-1);
            DisposeInput(playerIndex);
        }
        #endregion

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).started += LoadResurrection;
            UIMap.FindAction(c.UI.Submit.name).canceled += StopResurrectionLoading;
            _actionKey = UIMap.FindAction(c.UI.Submit.name).GetBindingDisplayString();

            SetUpSignifiers();
        }

        public override void SetUpSignifiers()
        {
            base.SetUpSignifiers();

            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(PlayerIndex, ActionMap.UI);

            // display main action signifiers
            int reviveEnergyIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Submit.name, PlayerIndex);
            string reviveEnergyActionKey = Helper.GetInputIcon(reviveEnergyIconIndex);

            _actionSignifiersController.DisplaySignifier(ACTION_TEXT, reviveEnergyActionKey);
        }

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            if (signifierController is not UIActionSignifiersController)
            {
                Debug.LogError($"Type mismatching for the action signifiers controller at {gameObject.name}");
                return;
            }

            _actionSignifiersController = (UIActionSignifiersController)signifierController;
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).started -= LoadResurrection;
            UIMap.FindAction(c.UI.Submit.name).canceled -= StopResurrectionLoading;

            _actionSignifiersController.ClearChildSignifiers();
        }
        #endregion

        private void SetReviveText()
        {
            int playerNumber = GameManager.Instance.PlayersManager.GetInactivePlayerIndex() + 1;
            _resurrectionActionText.text = $"{REVIVE_TEXT} {playerNumber} {REVIVE_ACTION_TEXT} {_actionKey}";
        }

        private void LoadResurrection(InputAction.CallbackContext _)
        {
            StopAllCoroutines();

            //if (!PlayerHasEnoughCoins())
            //{
            //    CancelInvoke();
            //    _coinsWarningText.enabled = true;
            //    Invoke(nameof(DisableCoinsWarningText), WARNING_DURATION);
            //    return;
            //}

            if(!IsPlayerDead())
            {
                // there are no dead players
                return;
            }

            StartCoroutine(LoadingProcess());
        }

        private void DisableCoinsWarningText()
        {
            _coinsWarningText.enabled = false;
        }

        private IEnumerator LoadingProcess()
        {
            float time = _processBar.fillAmount * _loadDuration;

            while(time < _loadDuration)
            {
                time += Time.deltaTime;
                float t = time / _loadDuration;
                _processBar.fillAmount = t;
                yield return null;
            }

            OnPlayerRevived();
        }

        private int _healthAmount;

        private void OnPlayerRevived()
        {
            _revived = true;
            _resurrectionActionText.text = NO_REVIVAL_MESSAGE;
            _detailsText.enabled = false;
            _processBar.fillAmount = 0f;

            // deduct cash
            //GameManager.Instance.PlayersManager.Coins.AddCoins(-Constants.REVIVAL_COST);
            //_coinsText.text = GameManager.Instance.PlayersManager.Coins.CoinsAmount.ToString();
            PlayerHealth playerHealth = GameManager.Instance.PlayersManager.GetPlayers(true)[0].Health as PlayerHealth;
            int currentHP = playerHealth.GetHealthAmount();
            _healthAmount = Mathf.CeilToInt(currentHP / 2f);
            playerHealth.SetHealthAmount(_healthAmount);

            // subscribe so that the effect takes place only when the workshop is exitted
            _workshopInteractableArea.OnInteractorExit += RevivePlayer;
        }

        public void RevivePlayer()
        {
            _revived = false;
            // revive player
            //Vector3 respawnPosition = _workshopInteractableArea.transform.position;
            int deadPlayerIndex = GameManager.Instance.PlayersManager.GetInactivePlayerIndex();
            PlayerComponents deadPlayer = GameManager.Instance.PlayersManager.GetPlayers(false)[deadPlayerIndex];
            
            Vector3 respawnPosition = deadPlayer.MiniPlayerSpawner.GetMiniPlayerTransform().position;

            GameManager.Instance.PlayersManager.PlayerSpawner.RevivePlayer(deadPlayerIndex, respawnPosition, _healthAmount);
            _workshopInteractableArea.OnInteractorExit -= RevivePlayer;
        }

        private void StopResurrectionLoading(InputAction.CallbackContext _)
        {
            StopAllCoroutines();
            StartCoroutine(UnloadingProcess());
        }

        private IEnumerator UnloadingProcess()
        {
            while(_processBar.fillAmount > 0f)
            {
                _processBar.fillAmount -= Time.deltaTime;
                yield return null;
            }
        }

        private bool PlayerHasEnoughCoins()
        {
            return GameManager.Instance.PlayersManager.Coins.CoinsAmount >= Constants.REVIVAL_COST;
        }

        private bool IsPlayerDead()
        {
            return GameManager.Instance.PlayersManager.HasDeadPlayers() && !_revived;
        }
    }
}
