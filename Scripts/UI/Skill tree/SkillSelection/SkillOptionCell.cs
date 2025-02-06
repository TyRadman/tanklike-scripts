using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree.SkillSelection
{
    using UI.SkillTree;

    public class SkillOptionCell : UICell, ICellSelectable
    {
        [field: SerializeField] public SkillProfile SkillProfile { get; private set; }
        public List<SkillsConnectedCell> ConnectedCells = new List<SkillsConnectedCell>();

        [field: SerializeField, Header("References")] public RectTransform RectTransform { get; private set; }
        [SerializeField] private GameObject _overlayImage;
        [SerializeField] private Image _cellIconImage;
        [SerializeField] private Image _highlightImage;
        [SerializeField] private Image _progressBar;

        [Header("Animations")]
        [SerializeField] private Animator _animator;

        private readonly int _onHighlightKey = Animator.StringToHash("Highlight");
        private readonly int _onDehighlightKey = Animator.StringToHash("Dehighlight");
        private readonly int _onShowKey = Animator.StringToHash("Show");
        private readonly int _onHideKey = Animator.StringToHash("Hide");
        private readonly int _onSelectKey = Animator.StringToHash("Charge");
        private readonly int _onDeselectKey = Animator.StringToHash("ReleaseCharge");
        private readonly int _onFinishedCharge = Animator.StringToHash("OnChargeFinished");

        private readonly int _chargeSpeedKey = Animator.StringToHash("ChargeSpeed");
        private readonly int _releaseChargeSpeedKey = Animator.StringToHash("DechargeSpeed");

        public override void SetUp()
        {
            SkillProfile.SkillHolder.PopulateSkillProperties();
            SetIcon(SkillProfile.SkillHolder.GetIcon());

            _progressBar.fillAmount = 0f;

            if (SkillProfile == null)
            {
                return;
            }

            _overlayImage.SetActive(false);
            SetIcon(SkillProfile.SkillHolder.GetIcon());
        }

        public override void SetIcon(Sprite icon)
        {
            _cellIconImage.sprite = icon;
        }

        public override void Highlight()
        {
            _highlightImage.enabled = true;
            _animator.SetTrigger(_onHighlightKey);
        }

        public override void Unhighlight()
        {
            _highlightImage.enabled = false;
            _animator.SetTrigger(_onDehighlightKey);
        }

        public void DisableHighlightImage()
        {
            _highlightImage.enabled = false;
        }

        public void SetProgressAmount(float amount)
        {
            _progressBar.fillAmount = amount;
        }

        public float GetFillAmount()
        {
            return _progressBar.fillAmount;
        }

        public void PlayShowAnimation()
        {
            _animator.SetTrigger(_onShowKey);
        }

        public void PlayHideAnimation()
        {
            _animator.SetTrigger(_onHideKey);
        }

        public void PlaySelectAnimation()
        {
            _animator.SetTrigger(_onSelectKey);
        }

        public void PlayDeselectAnimation()
        {
            _animator.SetTrigger(_onDeselectKey);
        }

        public void SetChargeAnimationSpeed(float holdDuration)
        {
            _animator.SetFloat(_chargeSpeedKey, holdDuration);
        }

        public void SetReleaseChargeAnimationSpeed(float holdDuration)
        {
            _animator.SetFloat(_releaseChargeSpeedKey, holdDuration);
        }

        public void PlayChargeFinishedAnimation()
        {
            _animator.SetTrigger(_onFinishedCharge);
        }

        public void SetSkillProfile(SkillProfile skillProfile)
        {
            SkillProfile = skillProfile;

            SkillProfile.SkillHolder.PopulateSkillProperties();
            SetIcon(SkillProfile.SkillHolder.GetIcon());
        }

        public ICellSelectable Navigate(Direction direction)
        {
            throw new System.NotImplementedException();
        }
    }
}
