using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TankLike.Combat.Destructible;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private MapMakerSelector _selector;
        
        [Header("Save menu references")]
        [SerializeField] private GameObject _saveMenu;
        [SerializeField] private TMP_InputField _mapNameInputField;
        [SerializeField] private TextMeshProUGUI _wanringText;

        [Header("Level Dimension Input Fields")]
        [SerializeField] private MapMakerDimensionsTextBox _XInputField;
        [SerializeField] private MapMakerDimensionsTextBox _YInputField;

        [Header("Mirror Options")]
        [SerializeField] private List<Button> _mirrorButtons = new List<Button>();

        [Header("Brush Sizes")]
        [SerializeField] private MapMakerBrushSizeButton _brushSizeButton;

        private MapMakerManager _mapMaker;

        private const float MESSAGE_DISPLAY_DURATION = 3f;

        private void Awake()
        {
            SetUp();
        }

        public void SetUp()
        {
            _mapMaker = GetComponent<MapMakerManager>();
            _saveMenu.SetActive(false);

            _XInputField.SetUp(_selector);
            _YInputField.SetUp(_selector);
            _brushSizeButton.SetUp(_selector);
        }

        public void SetActiveTile(int typeNumber)
        {
            _mapMaker.Selector.SetTileToBuild((TileType)typeNumber);
            _mapMaker.Overlays.CurrentOverlayType = DestructableTag.None;
        }

        public void SetActiveOverlayTile(int typeNumber)
        {
            _mapMaker.Selector.SetOverLayToBuild((DestructableTag)typeNumber);
        }

        public void ClearTiles()
        {
            _mapMaker.ClearLevel();
        }

        public void DisplayMessage(string message)
        {
            _messageText.text = message;
            CancelInvoke();
            Invoke(nameof(HideMessage), MESSAGE_DISPLAY_DURATION);
        }

        private void HideMessage()
        {
            _messageText.text = string.Empty;
        }

        public void ShowSaveMenu(bool show)
        {
            _saveMenu.SetActive(show);
        }

        public string GetMapName()
        {
            return _mapNameInputField.text;
        }

        public void SetSaveMenuWarning(string warning)
        {
            _wanringText.text = warning;
            Invoke(nameof(ResetWarningText), 1f);
        }

        private void ResetWarningText()
        {
            _wanringText.text = string.Empty;
        }
    }
}
