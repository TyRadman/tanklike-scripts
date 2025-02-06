using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TankLike.LevelGeneration.Quests;

namespace TankLike.UI.Inventory
{
    public class QuestElement : MonoBehaviour
    {
        public enum QuestDisplayState
        {
            NEW, DONE, None
        }

        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _questText;
        [SerializeField] private TextMeshProUGUI _questProgressText;
        [SerializeField] private Image _progressionBarImage;
        [SerializeField] private GameObject _content;
        [HideInInspector] public Quest_SO CurrectQuest;
        [SerializeField] private TextMeshProUGUI _newText;

        public void SetQuestText(Quest_SO quest)
        {
            CurrectQuest = quest;
            _questText.text = quest.GetQuestString();
            _questProgressText.text = quest.GetQuestProgressString();
            _progressionBarImage.fillAmount = quest.GetProgressAmount();
        }

        public void UpdateProgress()
        {
            _questProgressText.text = CurrectQuest.GetQuestProgressString();
            _progressionBarImage.fillAmount = CurrectQuest.GetProgressAmount();
        }

        public void Clear()
        {
            _questText.text = string.Empty;
        }

        public void Enable(bool enable)
        {
            _content.SetActive(enable);
        }

        public void EnableNewText(bool enable)
        {
            _newText.enabled = enable;
        }
    }
}
