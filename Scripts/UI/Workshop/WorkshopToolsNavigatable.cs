using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.Sound;
using TankLike.UnitControllers;
using TankLike.Utils;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static TankLike.UnitControllers.TankTools;
using TMPro;
using TankLike.UI.Signifiers;

namespace TankLike.UI.Workshop
{
    public class WorkshopToolsNavigatable : Navigatable, IInput
    {
        [Header("References")]
        [SerializeField] private WorkshopToolHolder _toolHolderReference;
        [SerializeField] private Transform _toolHoldersParent;
        [SerializeField] private RectTransform _toolsParent;
        [SerializeField] private TextMeshProUGUI _toolNameText;
        [SerializeField] private TextMeshProUGUI _toolDescriptionText;
        [SerializeField] private Image _toolIcon; // or gif for how it's used
        [Header("Energy References")]
        [SerializeField] private Image _energyBarFillImage;
        [SerializeField] private ToolAmountBar _toolAmountBarReference;
        [SerializeField] private Transform _barsParent;
        [SerializeField] private AnimationCurve _barChargeCurve;
        private List<ToolAmountBar> _toolAmountBars = new List<ToolAmountBar>();
        [Header("Slider settings")]
        [SerializeField] private float _stepHeight = 100f;
        // The number of tool holders at the top and the bottom that are ignored when navigating.
        // This means that when navigating through the tool list, the parent transform (_toolsParent) will not move
        // until the selected tool is beyond these initial and final tool holders.
        [SerializeField] private int _toolsCountToIgnore = 3;
        [SerializeField] private float _interpolationSpeed = 5f;

        private List<WorkshopToolHolder> _toolsHolders = new List<WorkshopToolHolder>();
        private WorkshopToolHolder _selectedHolder;
        private Coroutine _holdersParentMovementCoroutine;
        // input related
        private Coroutine _onNavigationButtonHoldCoroutine;
        private Coroutine _onFillButtonHoldCoroutine;
        private Vector2 _energyConsumptionLimits;
        private bool _isChargingTool = false;
        private bool _canCharge = true;
        private WaitForSeconds _buttonRepeatWait = new WaitForSeconds(Constants.ButtonRepeatFrequency);
        private WaitForSeconds _preButtonRepeatWait = new WaitForSeconds(Constants.PreButtonRepeatWaitTime);

        private int _currentIndex = 0;
        private Vector2 _parentLimits;
        private PlayerTools _currentPlayerTools;
        private PlayerEnergy _currentPlayerEnergy;
        private List<ToolPack> _tools;
        private UIActionSignifiersController _actionSignifiersController;

        private const string FILL_ENERGY_ACTION = "Fill";
        private const float BAR_CHARGE_DURATION = 2f;
        private const float RECOVERY_TIME = 0.3f;

        public override void SetUp()
        {
            // TODO: call base.SetUp()?

            // Create the bars
            for (int i = 0; i < Constants.MaxToolsUsageCount; i++)
            {
                ToolAmountBar bar = Instantiate(_toolAmountBarReference, _barsParent);
                bar.DisableImage();
                // TODO: remove after making sure that it's not needed anymore
                //bar.transform.parent = null;
                _toolAmountBars.Add(bar);
            }
        }

        public override void Dispose()
        {
            // TODO: call base.Dispose()?

            // Destroy the bars

            _toolAmountBars.ForEach(b => Destroy(b.gameObject));
            _toolAmountBars.Clear();
        }

        public override void SetUpActionSignifiers(ISignifierController signifierController)
        {
            if(signifierController is not UIActionSignifiersController)
            {
                Debug.LogError($"Type mismatching for the action signifiers controller at {gameObject.name}");
                return;
            }

            _actionSignifiersController = (UIActionSignifiersController)signifierController;
        }

        #region Input
        public void SetUpInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);

            UIMap.FindAction(c.UI.Submit.name).started += FillSelectedTool;
            UIMap.FindAction(c.UI.Submit.name).canceled += StopFillingSelectedTool;

            UIMap.FindAction(c.UI.Navigate_Up.name).performed += NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed += NavigateDown;

            UIMap.FindAction(c.UI.Navigate_Up.name).started += HoldDownUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).started += HoldDownDown;
            UIMap.FindAction(c.UI.Navigate_Up.name).canceled += StopHoldDownAction;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled += StopHoldDownAction;

            SetUpSignifiers();
        }

        public override void SetUpSignifiers()
        {
            base.SetUpSignifiers();

            PlayerInputActions c = InputManager.Controls;

            int fillEnergyActionIconIndex = GameManager.Instance.InputManager.GetButtonBindingIconIndex(c.UI.Submit.name, PlayerIndex);
            _actionSignifiersController.DisplaySignifier(FILL_ENERGY_ACTION, Helper.GetInputIcon(fillEnergyActionIconIndex));
        }

        private void FillSelectedTool(InputAction.CallbackContext _)
        {
            // check if there is enough energy
            if(!PlayerHasEnoughEnergy() || _selectedHolder.CurrentTool.Tool.HasFullAmmunition())
            {
                // display some warning

                return;
            }

            CheckForRunningCoroutine(_onFillButtonHoldCoroutine);
            _onFillButtonHoldCoroutine = StartCoroutine(FillHoldProcess());
        }

        private IEnumerator FillHoldProcess()
        {
            ToolPack tool = _selectedHolder.CurrentTool;

            if (!PlayerHasEnoughEnergy() || tool.Tool.HasFullAmmunition() || !_canCharge)
            {
                yield break;
            }

            float timer = 0f;
            ToolAmountBar bar = _toolAmountBars[tool.Tool.GetAmount()];
            _energyConsumptionLimits = new Vector2(_currentPlayerEnergy.GetCurrentEnergy(), 
                _currentPlayerEnergy.GetCurrentEnergy() - tool.Info.UsageEnergyCost);
            print(_energyConsumptionLimits);
            _isChargingTool = true;

            while (timer < BAR_CHARGE_DURATION)
            {
                timer += Time.deltaTime;
                float t = _barChargeCurve.Evaluate(timer / BAR_CHARGE_DURATION);
                bar.SetFillAmount(t);
                _currentPlayerEnergy.SetEnergyAmount(_energyConsumptionLimits.Lerp(t));
                UpdateEnergyBar();
                yield return null;
            }

            _isChargingTool = false;
            tool.Tool.AddAmount(1);
            //yield return new WaitForSeconds(0.1f);
            _selectedHolder.UpdateCount();
            _currentPlayerEnergy.SetEnergyAmount(_energyConsumptionLimits.y);
            UpdateEnergyBar();
            _onFillButtonHoldCoroutine = StartCoroutine(FillHoldProcess());
        }

        private void StopFillingSelectedTool(InputAction.CallbackContext _)
        {
            CheckForRunningCoroutine(_onFillButtonHoldCoroutine);

            if (_isChargingTool)
            {
                StartCoroutine(RecoverValuesProcess());
            }
        }

        private IEnumerator RecoverValuesProcess()
        {
            _isChargingTool = false;
            _canCharge = false;
            float timer = 0f;
            ToolAmountBar bar = _toolAmountBars[_selectedHolder.CurrentTool.Tool.GetAmount()];
            _energyConsumptionLimits = new Vector2(_currentPlayerEnergy.GetCurrentEnergy(), _energyConsumptionLimits.x);
            Vector2 barFillLimits = new Vector2(bar.GetFillAmount(), 0f);

            while (timer < RECOVERY_TIME)
            {
                timer += Time.deltaTime;
                float t = timer / RECOVERY_TIME;
                _currentPlayerEnergy.SetEnergyAmount(_energyConsumptionLimits.Lerp(t));
                bar.SetFillAmount(barFillLimits.Lerp(t));
                UpdateEnergyBar();
                yield return null;
            }

            _currentPlayerEnergy.SetEnergyAmount(_energyConsumptionLimits.y);
            _toolAmountBars[_selectedHolder.CurrentTool.Tool.GetAmount()].SetFillAmount(0f);
            UpdateEnergyBar();
            _canCharge = true;
        }

        public override void NavigateUp(InputAction.CallbackContext _)
        {
            Navigate(Direction.Up);
        }

        public override void NavigateDown(InputAction.CallbackContext _)
        {
            Navigate(Direction.Down);
        }

        private void HoldDownUp(InputAction.CallbackContext _)
        {
            CheckForRunningCoroutine(_onNavigationButtonHoldCoroutine);
            _onNavigationButtonHoldCoroutine = StartCoroutine(HoldDownProcess(Direction.Up));
        }

        private void HoldDownDown(InputAction.CallbackContext _)
        {
            CheckForRunningCoroutine(_onNavigationButtonHoldCoroutine);
            _onNavigationButtonHoldCoroutine = StartCoroutine(HoldDownProcess(Direction.Down));
        }

        private void StopHoldDownAction(InputAction.CallbackContext _)
        {
            CheckForRunningCoroutine(_onNavigationButtonHoldCoroutine);
        }

        private IEnumerator HoldDownProcess(Direction direction)
        {
            yield return _preButtonRepeatWait;

            while (true)
            {
                Navigate(direction);
                yield return _buttonRepeatWait;
            }
        }

        public void DisposeInput(int playerIndex)
        {
            PlayerInputActions c = InputManager.Controls;
            InputActionMap UIMap = InputManager.GetMap(playerIndex, ActionMap.UI);
            UIMap.FindAction(c.UI.Submit.name).started -= FillSelectedTool;
            UIMap.FindAction(c.UI.Submit.name).canceled -= StopFillingSelectedTool;
            UIMap.FindAction(c.UI.Navigate_Up.name).performed -= NavigateUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).performed -= NavigateDown;

            UIMap.FindAction(c.UI.Navigate_Up.name).started -= HoldDownUp;
            UIMap.FindAction(c.UI.Navigate_Down.name).started -= HoldDownDown;
            UIMap.FindAction(c.UI.Navigate_Up.name).canceled -= StopHoldDownAction;
            UIMap.FindAction(c.UI.Navigate_Down.name).canceled -= StopHoldDownAction;

            _actionSignifiersController.ClearChildSignifiers();
        }
        #endregion

        public override void Open(int playerIndex)
        {
            base.Open(playerIndex);

            SetUpInput(playerIndex);

            CachePlayerTools();

            CreateNeededToolHolders();

            ConnectHoldersInput();

            FillToolHoldersData();

            DisableUnusedToolHolders();

            HighlightFirstHolder();

            SetUpValues();

            UpdateToolInfo();
        }

        public override void Close(int playerIndex)
        {
            base.Close(playerIndex);
            DisposeInput(PlayerIndex);
        }

        private void CachePlayerTools()
        {
            PlayerComponents player = GameManager.Instance.PlayersManager.GetPlayer(PlayerIndex);
            _currentPlayerTools = player.Tools;
            _tools = _currentPlayerTools.GetTools();
            _currentPlayerEnergy = player.Energy;
        }

        private void SetUpValues()
        {
            // set the movement limits of the holders parent (the slider)
            _parentLimits = new Vector2(0f, Mathf.Max(0f, _stepHeight * (_toolsHolders.Count - _toolsCountToIgnore * 2)));

            // set energy bar values
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            _energyBarFillImage.fillAmount = _currentPlayerEnergy.GetEnergyPercentage();
        }

        /// <summary>
        /// Creates more tool holders if any are needed (depending on the number of tools the player possesses)
        /// </summary>
        private void CreateNeededToolHolders()
        {
            int neededToolHoldersCount = _tools.Count - _toolsHolders.Count;

            for (int i = 0; i < neededToolHoldersCount; i++)
            {
                WorkshopToolHolder holder = Instantiate(_toolHolderReference, _toolHoldersParent);
                _toolsHolders.Add(holder);
            }
        }

        private void ConnectHoldersInput()
        {
            for (int i = 0; i < _toolsHolders.Count; i++)
            {
                WorkshopToolHolder currentHolder = _toolsHolders[i];
                WorkshopToolHolder nextHolder = _toolsHolders[(i + 1) % _toolsHolders.Count];
                WorkshopToolHolder previousHolder = _toolsHolders[i - 1 < 0? _toolsHolders.Count - 1 : i - 1];

                ConnectHolderToHolderWithDirection(currentHolder, nextHolder, Direction.Down);
                ConnectHolderToHolderWithDirection(nextHolder, currentHolder, Direction.Up);
                ConnectHolderToHolderWithDirection(currentHolder, previousHolder, Direction.Up);
                ConnectHolderToHolderWithDirection(previousHolder, currentHolder, Direction.Down);
            }
        }

        private void ConnectHolderToHolderWithDirection(WorkshopToolHolder baseHolder, WorkshopToolHolder holderToConnectTo, Direction direction)
        {
            UnityEvent currentDirectionAction = baseHolder.MenuItem.GetActions().Find(a => a.Direction == direction).Action;
            currentDirectionAction.RemoveAllListeners();
            currentDirectionAction.AddListener(() => HighlightHolder(holderToConnectTo));
        }

        private void FillToolHoldersData()
        {
            for (int i = 0; i < _tools.Count; i++)
            {
                _toolsHolders[i].SetUp(_tools[i]);
            }
        }

        private void DisableUnusedToolHolders()
        {
            // if there are no tool holders to disable then skip 
            if (_toolsHolders.Count - _tools.Count < 1)
            {
                return;
            }

            for (int i = _tools.Count; i < _toolsHolders.Count; i++)
            {
                _toolsHolders[i].ResetHolder();
            }
        }

        private void HighlightFirstHolder()
        {
            if (_selectedHolder != null)
            {
                _selectedHolder.Highlight(false);
            }

            _currentIndex = 0;
            _selectedHolder = _toolsHolders[_currentIndex];
            _selectedHolder.Highlight(true);
        }

        public override void Navigate(Direction direction)
        {
            AudioManager audioManager = GameManager.Instance.AudioManager;
            audioManager.Play(audioManager.UIAudio.NavigateMenuAudio);
         
            _selectedHolder.MenuItem.InvokeAction(direction);
            _currentIndex += direction == Direction.Up ? -1 : 1;
            _currentIndex = (_currentIndex + _toolsHolders.Count) % _toolsHolders.Count;
            UpdateToolInfo();

            if (_holdersParentMovementCoroutine != null)
            {
                StopCoroutine(_holdersParentMovementCoroutine);
            }

            _holdersParentMovementCoroutine = StartCoroutine(UpdateParentPosition());
        }

        private IEnumerator UpdateParentPosition()
        {
            float t = Mathf.InverseLerp(_toolsCountToIgnore, _toolsHolders.Count - _toolsCountToIgnore - 1, _currentIndex);
            float newHeight = _parentLimits.Lerp(t);

            while (Mathf.Abs(_toolsParent.anchoredPosition.y - newHeight) > 0.1f)
            {
                _toolsParent.anchoredPosition = Vector2.up * Mathf.Lerp(_toolsParent.anchoredPosition.y, newHeight, _interpolationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private void UpdateToolInfo()
        {
            ToolPack tool = _selectedHolder.CurrentTool;
            _toolNameText.text = tool.Info.Name;
            _toolDescriptionText.text = tool.Info.Description;
            _toolIcon.sprite = tool.Info.IconImage;

            // update the bars
            int toolMaxAmount = tool.Tool.GetMaxAmount();

            for (int i = 0; i < toolMaxAmount; i++)
            {
                ToolAmountBar bar = _toolAmountBars[i];
                bar.EnableImage();
                bar.SetFillAmount(0f);
                bar.transform.parent = _barsParent;
            }

            // fill the filled ones
            int barsToFillCount = tool.Tool.GetAmount();

            for (int i = 0; i < barsToFillCount; i++)
            {
                ToolAmountBar bar = _toolAmountBars[i];
                bar.SetFillAmount(1f);
            }

            for (int i = toolMaxAmount; i < Constants.MaxToolsUsageCount; i++)
            {
                _toolAmountBars[i].DisableImage();
                _toolAmountBars[i].transform.parent = null;
            }
        }

        public void HighlightHolder(WorkshopToolHolder holderToHighlight)
        {
            _selectedHolder.Highlight(false);
            _selectedHolder = holderToHighlight;
            _selectedHolder.Highlight(true);
        }

        #region Extra
        private bool PlayerHasEnoughEnergy()
        {
            return _currentPlayerEnergy.GetCurrentEnergy() >= _selectedHolder.CurrentTool.Info.UsageEnergyCost;
        }

        private void CheckForRunningCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        #endregion
    }
}
