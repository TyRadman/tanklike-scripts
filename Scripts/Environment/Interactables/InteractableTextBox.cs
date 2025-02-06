using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.Environment
{
    public class InteractableTextBox : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _interactionText;
        [SerializeField] private TextMeshPro _feedbackText;
        [SerializeField] private Animation _anim;
        [SerializeField] private AnimationClip _openAnimation;
        [SerializeField] private AnimationClip _closeAnimation;
        
        private Transform _cam;
        private bool _isOpened = false;

        private void Start()
        {
            _cam = Camera.main.transform;
        }

        void Update()
        {
            // so that the text only rotates on the x-axis
            Vector3 targetPostition = new Vector3(transform.position.x, _cam.position.y, _cam.position.z);
            transform.LookAt(targetPostition);
        }

        public void SetInteractionText(string text)
        {
            _interactionText.text = text;
            _feedbackText.text = string.Empty;
        }

        public void SetFeedbackText(string text)
        {
            _feedbackText.text = text;
        }

        public void PlayOpenAnimation(bool open)
        {
            // if the box is opened, then there's no need to open it again. If it's closed, then there is no need to close it again
            if(_isOpened == open)
            {
                return;
            }

            _isOpened = open;

            if (_anim.isPlaying)
            {
                _anim.Stop();
            }

            _anim.clip = open ? _openAnimation : _closeAnimation;
            _anim.Play();
        }

        public void SetPosition(Transform parent)
        {
            transform.position = parent.position;
        }
    }
}
