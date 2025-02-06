using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TankLike.Sound;

namespace TankLike.UI.Notifications
{
    public class QuestNotificationBar : MonoBehaviour
    {
        public enum QuestState
        {
            Started = 0, InProgress = 1, Finished = 2, Failed = 3
        }

        [SerializeField] private TextMeshProUGUI _mainText;
        [SerializeField] private TextMeshProUGUI _questTypeText;
        [SerializeField] private Animation _anim;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;
        [SerializeField] private Audio _questStartAudio;
        [SerializeField] private Audio _questEndAudio;
        private const string QUEST_COMPLETED_MESSAGE = "Quest Completed";
        private const string QUEST_STARTED_MESSAGE = "Quest Completed";

        public void Display(string message, QuestState state)
        {
            _mainText.text = message;

            CancelInvoke();
            PlayAnimation(_showClip);
            //_questAudio.Play(); //temporary

            switch (state)
            {
                case QuestState.Started:
                    {
                        GameManager.Instance.AudioManager.Play(_questStartAudio);
                        break;
                    }
                case QuestState.Finished:
                    {
                        GameManager.Instance.AudioManager.Play(_questEndAudio);
                        OnFinished();
                        break;
                    }
            }
        }

        public void Hide()
        {
            PlayAnimation(_hideClip);
        }

        public void OnFinished()
        {
            _questTypeText.text = QUEST_COMPLETED_MESSAGE;
            // add a strikethrough to the text
            _mainText.fontStyle = FontStyles.Strikethrough;
        }

        private void OnStarted()
        {
            _questTypeText.text = QUEST_STARTED_MESSAGE;
        }

        private void PlayAnimation(AnimationClip clip)
        {
            if (_anim.isPlaying) _anim.Stop();

            _anim.clip = clip;
            _anim.Play();
        }
    }
}
