using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TankLike.UI
{
    using Utils;

    public class CoinsHUD : MonoBehaviour, IManager
    {
        [SerializeField] private Animation _animation;
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private AnimationClip _displayAnimationClip;
        [SerializeField] private AnimationClip _hideAnimationClip;

        private Coroutine _displayRoutine;

        private WaitForSeconds _displayWait = new WaitForSeconds(3f);

        public bool IsActive { get; private set; }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _coinsText.text = "0";
        }

        public void Dispose()
        {
            IsActive = false;
        }
        #endregion

        public void DisplayCoinsText(int coinsAmount)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            CancelInvoke();

            _coinsText.text = $"{coinsAmount}";

            if(_displayRoutine == null)
            {
                this.PlayAnimation(_animation, _displayAnimationClip);
            }

            this.StopCoroutineSafe(_displayRoutine);

            _displayRoutine = StartCoroutine(DisplayRoutine());
        }

        private IEnumerator DisplayRoutine()
        {
            yield return _displayWait;

            this.PlayAnimation(_animation, _hideAnimationClip);
            _displayRoutine = null;
        }
    }
}
