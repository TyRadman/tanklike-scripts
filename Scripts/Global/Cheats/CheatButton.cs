using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.Cheats
{
    public class CheatButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        private static Color EnabledColor = new Color(0.75f, 0.9502094f, 1f, 1f);
        private static Color DisabledColor = new Color(1f, 1f, 1f, 1f);
        private CheatsManager _cheatsManager;
        private CheatCell _cheat;
        [SerializeField] private Image _arrowImage;

        private void Awake()
        {
        }

        public void SetUp(CheatsManager manager)
        {
            _arrowImage.enabled = false;
            _cheatsManager = manager;
        }

        public void AssignCheat(CheatCell cheat)
        {
            _cheat = cheat;
            _button.onClick.RemoveAllListeners();

            if (cheat is CheatsParent)
            {
                _arrowImage.enabled = true;
                _button.onClick.AddListener(OpenCheatsParent);
            }
            else
            {
                _arrowImage.enabled = false;
                _button.onClick.AddListener(cheat.PerformCheat);
            }

            _text.text = cheat.CheatText;
            cheat.Button = this;
        }

        public void SetButtonColor(bool enable)
        {
            _image.color = enable ? EnabledColor : DisabledColor;
        }

        private void OpenCheatsParent()
        {
            _cheatsManager.BindButtonsToCheats(_cheat as CheatsParent);
        }
    }
}
