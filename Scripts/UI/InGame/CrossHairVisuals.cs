using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.InGame
{
    public class CrossHairVisuals : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _innerLayer;
        [SerializeField] private SpriteRenderer _outerLayer;
        [SerializeField] private SegmentedBar _shootingBar;

        [Header("Aim Visuals")]
        [SerializeField] private GameObject _innerAim;
        [SerializeField] private GameObject _outerAim;

        private readonly int _showTriggerHash = Animator.StringToHash("Show");
        private readonly int _hideTriggerHash = Animator.StringToHash("Hide");
        private readonly int _shootTriggerHash = Animator.StringToHash("Shoot");
        private readonly int _rechargeHash = Animator.StringToHash("ShotRecharge");
        private readonly int _isAimingHash = Animator.StringToHash("IsAiming");
        private readonly int _isVisible = Animator.StringToHash("IsVisible");

        public void SetUp()
        {
            _innerAim.SetActive(false);
            _outerAim.SetActive(false);
        }

        public void SetColor(Color color, Color holdColor)
        {
            // TODO: do we give hold down bar a different color?
            //_innerLayer.color = holdColor;
            //_outerLayer.color = color;
            _shootingBar.SetColor(color);
            _innerAim.GetComponent<MeshRenderer>().material.color = color;
            _outerAim.GetComponent<MeshRenderer>().material.color = color;
        }

        public void ShowCrossHair()
        {
            TriggerAnimation(_showTriggerHash);
            SetIsVisible(true);
        }

        public void HideCrossHair()
        {
            TriggerAnimation(_hideTriggerHash);
            SetIsVisible(false);
        }

        public void PlayShootAnimation()
        {
            TriggerAnimation(_shootTriggerHash);
        }

        public void PlayOnShotReloadAnimation()
        {
            TriggerAnimation(_rechargeHash);
        }

        public void PlayActiveAimAnimation()
        {
            _innerAim.SetActive(true);
            _outerAim.SetActive(true);
            _animator.SetBool(_isAimingHash, true);
        }        
        
        public void PlayInactiveAimAnimation()
        {
            _innerAim.SetActive(true);
            _outerAim.SetActive(false);
            _animator.SetBool(_isAimingHash, false);
        }

        public void StopAiming()
        {
            _animator.SetBool(_isAimingHash, false);
            _innerAim.SetActive(false);
            _outerAim.SetActive(false);
        }

        private void TriggerAnimation(int triggerID)
        {
            _animator.SetTrigger(triggerID);
        }

        public void SetIsVisible(bool isVisible)
        {
            _animator.SetBool(_isVisible, isVisible);
        }
    }
}
