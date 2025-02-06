using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TankLike.UI.HUD
{
    public class HUDSkillIcon : MonoBehaviour, IPausable
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _keyText;
        [SerializeField] private Image _fillingImage;
        [SerializeField] private Image _cooldownFillImage;
        [Header("Animation")]
        [SerializeField] private Animation _skillAnimation;
        [SerializeField] private AnimationClip _onReadyAnimation;
        [SerializeField] private AnimationClip _readyLoopAnimation;
        private Material _iconMaterial;
        private bool _isCharged = false;

        private const string OVERLAY_EFFECT_WEIGHT_KEY = "_EffectWeight";

        public void SetUp()
        {
            _iconMaterial = Instantiate(_iconImage.material);
            _iconImage.material = _iconMaterial;
            StopPulseAnimation();
        }

        public void SetIconSprite(Sprite icon)
        {
            _iconImage.sprite = icon;
            _fillingImage.sprite = icon;
        }

        public void SetFillAmount(float amount)
        {
            _fillingImage.fillAmount = 1f;
            _cooldownFillImage.fillAmount = amount;
        }

        public void PlayReadyAnimation()
        {
            _fillingImage.fillAmount = 0f;
            _skillAnimation.clip = _onReadyAnimation;
            _skillAnimation.Play();
        }

        public void SetKey(string key)
        {
            _keyText.text = key;
        }

        public void PlayPulseAnimation()
        {
            _isCharged = true;
            _iconMaterial.SetFloat(OVERLAY_EFFECT_WEIGHT_KEY, 1f);
            //_animator.SetTrigger(StartPulsingKey);

            _cooldownFillImage.fillAmount = 0f;
            _fillingImage.fillAmount = 0f;
            _skillAnimation.clip = _readyLoopAnimation;
            _skillAnimation.Play();
        }

        public void StopPulseAnimation()
        {
            _isCharged = false;
            _iconMaterial.SetFloat(OVERLAY_EFFECT_WEIGHT_KEY, 0f);
            //_animator.SetTrigger(StopPulsingKey);

            _fillingImage.fillAmount = 1f;
            _skillAnimation.clip = _onReadyAnimation;
            _skillAnimation.Play();
        }

        public void OnPaused()
        {
            
        }

        public void OnResumed()
        {
            if (_isCharged)
            {
                PlayPulseAnimation();
            }
        }
    }
}
