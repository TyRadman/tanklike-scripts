using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.LevelGeneration.Quests;
using static TankLike.UI.Inventory.QuestElement;

namespace TankLike.UI.Inventory
{
    public class QuestNavigator : Navigatable
    {
        // ideally, the number of the maximum quests available at any moment throught the game
        [SerializeField] private List<QuestElement> _questElements;
        /// <summary>
        /// The list for one level. Another list for all the game can be implemented somewhere else
        /// </summary>
        private List<Quest_SO> _currentLevelQuests = new List<Quest_SO>();
        [SerializeField] private Color _doneColor;
        [SerializeField] private Color _newColor;

        /// <summary>
        /// Called when the window is opened
        /// </summary>
        public override void SetUp()
        {
            base.SetUp();

            _questElements.ForEach(q => q.Enable(false));
            // subscribe to the quests managers
            GameManager.Instance.QuestsManager.OnQuestAdded += OnQuestAdded;
            GameManager.Instance.QuestsManager.OnQuestCompleted += OnQuestCompleted;
            GameManager.Instance.QuestsManager.OnQuestProgressed += OnQuestProgressed;
        }

        public override void Dispose()
        {
            base.Dispose();

            _currentLevelQuests.Clear();

            // unsubscribe to the quests managers
            GameManager.Instance.QuestsManager.OnQuestAdded -= OnQuestAdded;
            GameManager.Instance.QuestsManager.OnQuestCompleted -= OnQuestCompleted;
            GameManager.Instance.QuestsManager.OnQuestProgressed -= OnQuestProgressed;
        }

        public override void Close(int playerIndex = 0)
        {
            base.Close(playerIndex);

            _questElements.ForEach(q => q.EnableNewText(false));
        }

        /// <summary>
        /// Whenever the player gets a new quest
        /// </summary>
        private void OnQuestAdded(Quest_SO quest)
        {
            _currentLevelQuests.Add(quest);
            ArrangeQuestsBasedOnState();
        }

        private void OnQuestCompleted(Quest_SO quest)
        {
            // just re-arrange the elements and it will recolor them and put them in order
            ArrangeQuestsBasedOnState();
        }

        private void OnQuestProgressed(Quest_SO quest)
        {
            _questElements.Find(q => q.CurrectQuest == quest).UpdateProgress();
        }

        private void ArrangeQuestsBasedOnState()
        {
            List<Quest_SO> doneQuests = _currentLevelQuests.FindAll(q => !q.IsActive);
            List<Quest_SO> undoneQuests = _currentLevelQuests.FindAll(q => q.IsActive);

            int i = 0;

            for (; i < undoneQuests.Count; i++)
            {
                _questElements[i].Enable(true);
                _questElements[i].SetQuestText(undoneQuests[i]);
            }

            for (; i < undoneQuests.Count + doneQuests.Count; i++)
            {
                _questElements[i].Enable(true);
                _questElements[i].SetQuestText(doneQuests[i - undoneQuests.Count]);
            }

            // disable the rest of the elements
            for (; i < _questElements.Count; i++)
            {
                _questElements[i].Enable(false);
            }
        }
    }
}
