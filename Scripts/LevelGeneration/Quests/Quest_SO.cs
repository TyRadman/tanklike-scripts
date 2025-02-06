using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.LevelGeneration.Quests
{
    public abstract class Quest_SO : ScriptableObject
    {
        public bool IsActive = true;
        public bool IsNew = true;

        public virtual void SetUp(LevelQuestSettings settings)
        {
            IsActive = true;
            IsNew = true;
        }

        /// <summary>
        /// This is a raw method, every child of this class must implement a custom OnProgress that calls this one at the end
        /// </summary>
        public void OnProgress(Quest_SO quest)
        {
            GameManager.Instance.QuestsManager.ReportProgress(quest);

            if (IsCompleted())
            {
                OnCompletion();
            }
        }

        public virtual void OnCompletion()
        {

        }

        public virtual string GetQuestString()
        {
            return string.Empty;
        }

        public virtual string GetQuestProgressString()
        {
            return string.Empty;
        }

        public virtual float GetProgressAmount()
        {
            return 3f;
        }

        public virtual bool IsCompleted()
        {
            return false;
        }
        
        public virtual void CopyValuesTo(Quest_SO quest)
        {

        }
    }
}
