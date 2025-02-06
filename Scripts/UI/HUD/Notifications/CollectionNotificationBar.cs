using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.Notifications
{
    using Utils;

    public class CollectionNotificationBar : MonoBehaviour
    {
        public NotificationBarSettings_SO Data { get; private set; }
        public bool IsAvailable { get; private set; }
        public bool CanAddTo { get; private set; } = true;

        public RectTransform Rect;
        //public NotificationType Type { get; private set; }
        
        [SerializeField] private TextMeshProUGUI _notificationAmountText;
        [SerializeField] private TextMeshProUGUI _notificationDescriptionText;
        [SerializeField] private Image _notificationIcon;

        [Header("Animations")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;

        private Coroutine _countDownCoroutine;
        private WaitForSeconds _countDownWait = new WaitForSeconds(4);
        private int _itemAmount = 0;

        public void SetUp()
        {
            IsAvailable = true;
        }

        public void Dispose()
        {
            _itemAmount = 0;

            if(_countDownCoroutine != null)
            {
                StopCoroutine(_countDownCoroutine);
            }

            this.PlayAnimation(_animation, _hideClip);
            IsAvailable = true;
            CanAddTo = true;
        }

        public void Display(NotificationBarSettings_SO data, int amount)
        {
            Data = data;

            _itemAmount += amount;

            _notificationAmountText.text = _itemAmount == 0 ? string.Empty : $"+{_itemAmount}";
            _notificationAmountText.color = data.AmountColor;
            _notificationDescriptionText.text = data.Name;

            _notificationIcon.color = data.AmountColor;
            _notificationIcon.sprite = data.Icon;
            //Type = data.Type;

            if (IsAvailable)
            {
                IsAvailable = false;
                this.PlayAnimation(_animation, _showClip);
                transform.SetAsLastSibling();
            }

            this.StopCoroutineSafe(_countDownCoroutine);

            _countDownCoroutine = StartCoroutine(CountDownProcess());
        }

        private IEnumerator CountDownProcess()
        {
            yield return _countDownWait;

            _itemAmount = 0;
            this.PlayAnimation(_animation, _hideClip);
            IsAvailable = true;
            CanAddTo = true;
        }
    }
}
