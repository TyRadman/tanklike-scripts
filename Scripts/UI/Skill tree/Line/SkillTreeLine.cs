using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree
{
    using Utils;

    public class SkillTreeLine : MonoBehaviour, IPresetable
    {
        [SerializeField] private Image _lineImage;
        [field: SerializeField] public RectTransform LineTransfrom { get; private set; }
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _unlockedClip;
        [SerializeField] private AnimationClip _lockedClip;

        [Header("Presets")]
        [SerializeField] private SkillTreeLinePreset _onUnlockedPreset;
        [SerializeField] private SkillTreeLinePreset _onLockedPreset;

        [Header("Preset References")]
        [SerializeField] private CanvasGroup _unlockLineCanvasGroup;
        [SerializeField] private Transform _unlockLineTransform;
        [SerializeField] private CanvasGroup _lockLineCanvasGroup;
        [SerializeField] private Transform _lockLineTransform;

        private System.Action _onEnabled;

        public void PlayLockedAnimation()
        {
            this.PlayAnimation(_animation, _lockedClip);

            _onEnabled += () => ApplyPreset(_onLockedPreset);

            Invoke(nameof(ClearOnEnabled), _lockedClip.length);
        }

        public void PlayUnlockedAnimation()
        {
            this.PlayAnimation(_animation, _unlockedClip);

            _onEnabled += () => ApplyPreset(_onUnlockedPreset);

            Invoke(nameof(ClearOnEnabled), _lockedClip.length);
        }

        private void ClearOnEnabled()
        {
            _onEnabled?.Invoke();
            _onEnabled = null;
        }

        public void ConnectCells(SkillTreeCell cell, SkillTreeCell nextCell)
        {
            nextCell.AddPreviousCellAndLine(this);

            Vector2 startCellPosition = cell.RectTransform.anchoredPosition;
            Vector2 endCellPosition = nextCell.RectTransform.anchoredPosition;

            LineTransfrom.anchoredPosition = (endCellPosition + startCellPosition) / 2;

            Vector2 direction = endCellPosition - startCellPosition;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 270f;
            LineTransfrom.localRotation = Quaternion.Euler(0f, 0f, angle);

            float distance = Vector2.Distance(startCellPosition, endCellPosition);
            LineTransfrom.sizeDelta = new Vector2(LineTransfrom.sizeDelta.x, distance);
        }

        public void ApplyPreset(IPreset preset)
        {
            if (preset is SkillTreeLinePreset linePreset)
            {
                _unlockLineTransform.localScale = linePreset.UnlockingLineScale;
                _unlockLineCanvasGroup.alpha = linePreset.UnlockingLineAlpha;

                _lockLineTransform.localScale = linePreset.LockingLineScale;
                _lockLineCanvasGroup.alpha = linePreset.LockingLineAlpha;
            }
        }
    }
}
