using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.LevelGeneration.Quests
{
    using Utils;
    using Combat.Destructible;

    [CreateAssetMenu(fileName = "DestroyObjectsQuest", menuName = "Other/ Quest Types/ Destroy Objects")]
    public class DestroyObjects_Quest : Quest_SO
    {
        [SerializeField] private Vector2Int _objectCountRange = new Vector2Int() { x = 2, y = 7 };
        private int _currentDestroyedObjectCount;
        private int _requiredObjectCount;
        [SerializeField] private DropperTag _destructableTag;

        public override void SetUp(LevelQuestSettings settings)
        {
            base.SetUp(settings);

            _currentDestroyedObjectCount = 0;
            _requiredObjectCount = _objectCountRange.RandomValue();
            GameManager.Instance.ReportManager.OnObjectDestroyed += OnProgress;
        }

        public void OnProgress(DropperTag tag, int playerIndex)
        {
            if (tag != _destructableTag)
            {
                return;
            }

            _currentDestroyedObjectCount++;

            OnProgress(this);
        }

        public override void OnCompletion()
        {
            base.OnCompletion();

            GameManager.Instance.ReportManager.OnObjectDestroyed -= OnProgress;
            // remove the quest from the quest manager
            GameManager.Instance.QuestsManager.MarkQuestAsCompleted(this);
        }

        public override string GetQuestString()
        {
            return $"Destroy {_requiredObjectCount.ToString().Color(Color.red)} {_destructableTag}{(_requiredObjectCount > 1 ? "s" : "")}";
        }

        public override string GetQuestProgressString()
        {
            return $"{_currentDestroyedObjectCount} / {_requiredObjectCount}";
        }

        public override float GetProgressAmount()
        {
            return (float)_currentDestroyedObjectCount / (float)_requiredObjectCount;
        }

        public override bool IsCompleted()
        {
            return _currentDestroyedObjectCount >= _requiredObjectCount;
        }

        public override void CopyValuesTo(Quest_SO quest)
        {
            DestroyObjects_Quest newQuest = (DestroyObjects_Quest)(quest);
            newQuest.SetValues(_destructableTag, _objectCountRange);
        }

        public void SetValues(DropperTag tag, Vector2Int objectsCountRange)
        {
            _destructableTag = tag;
            _objectCountRange = objectsCountRange;
        }
    }
}
