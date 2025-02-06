using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TankLike.UI.Notifications.QuestNotificationBar;

namespace TankLike.UI.Notifications
{
    public class NotificationsManager : MonoBehaviour, IManager
    {

        [System.Serializable]
        public class QuestTask
        {
            public string Message;
            public QuestState State;
        }

        [Header("Quests")]
        [SerializeField] private QuestNotificationBar _questBar;
        private const float QUEST_ON_DISPLAY_SHORT_DURATION = 1f;
        private const float QUEST_ON_DISPLAY_LONG_DURATION = 3f;
        private const float QUEST_HIDING_DURATION = 1f;
        private WaitForSeconds _questShortWait;
        private WaitForSeconds _questLongWait;
        private WaitForSeconds _questHideWait;
        [SerializeField] private List<PlayerNotificationController> _playerNotifications;
        [SerializeField] private List<QuestTask> _questTasks = new List<QuestTask>();

        public bool IsActive { get; private set; }

        public void SetReferences()
        {
            _questShortWait = new WaitForSeconds(QUEST_ON_DISPLAY_SHORT_DURATION);
            _questLongWait = new WaitForSeconds(QUEST_ON_DISPLAY_LONG_DURATION);
            _questHideWait = new WaitForSeconds(QUEST_HIDING_DURATION);
        }

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _playerNotifications.ForEach(p => p.SetUp());
        }

        public void Dispose()
        {
            IsActive = false;

            StopAllCoroutines();

            _questBar.Hide();

            _playerNotifications.ForEach(p => p.Dispose());
            _questTasks.Clear();
        }
        #endregion


        public void PushCollectionNotification(NotificationBarSettings_SO settings, int amount, int playerIndex)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            _playerNotifications[playerIndex].PushCollectionNotification(amount, settings);
        }

        #region Quest Notification
        public void PushQuestNotification(string _message, QuestState type)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            QuestTask task = new QuestTask() { Message = _message, State = type };
            _questTasks.Add(task);

            if (_questTasks.Count == 1)
            {
                StartCoroutine(PerformingTaskProcess(task));
            }
        }

        private IEnumerator PerformingTaskProcess(QuestTask task)
        {
            _questBar.Display(task.Message, task.State);

            yield return new WaitForEndOfFrame();

            // if this is the last task, then display it twice as long, otherwise, make it short
            if (_questTasks.Count == 1)
            {
                yield return _questLongWait;
            }
            else
            {
                yield return _questShortWait;
            }

            _questBar.Hide();
            yield return _questHideWait;
            // remove the task from the task list
            _questTasks.Remove(task);

            if (_questTasks.Count > 0)
            {
                StartCoroutine(PerformingTaskProcess(_questTasks[0]));
            }
        }
        #endregion
    }
}
