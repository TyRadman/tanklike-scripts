using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;

namespace TankLike.Combat.SkillTree
{
    public class SkillTreeUIDisplayer : MonoBehaviour
    {
        [SerializeField] private Color _unavailableCellColor;
        [SerializeField] private Color _lockedCellColor;
        [SerializeField] private Color _unlockedCellColor;

        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _requiredSkillPointsText;
        [SerializeField] private TextMeshProUGUI _playerSkillPointsText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private CanvasGroup _skillPointsTextParent;

        [Header("Skill State References")]
        [SerializeField] private TextMeshProUGUI _skillStateText;
        [SerializeField] private Image _skillStateIconImage;
        [SerializeField] private Sprite _unavailableIconSprite;
        [SerializeField] private Sprite _lockedIconSprite;
        [SerializeField] private Sprite _unlockedIconSprite;

        [Header("Video References")]
        [SerializeField] private Image _videoCoverImage;
        [SerializeField] private VideoPlayer _videoPlayer;

        public void UpdateName(string name)
        {
            _nameText.text = name;
        }

        public void UpdateDescription(string description)
        {
            _descriptionText.text = description;
        }

        public void UpdatePreviewVideo(VideoClip videoClip)
        {
            if (videoClip != null)
            {
                _videoCoverImage.enabled = false;
                _videoPlayer.clip = videoClip;
                _videoPlayer.Play();
            }
            else
            {
                _videoCoverImage.enabled = true;
            }
        }

        public void UpdatePlayerSkillPointsCount(int playerPoints)
        {
            _playerSkillPointsText.text = playerPoints.ToString();
        }

        public void UpdateSkillPoints(bool displayPoints, int playerPoints, int requiredPoints)
        {
            if (displayPoints)
            {
                _skillPointsTextParent.alpha = 1f;
            }
            else
            {
                _skillPointsTextParent.alpha = 0f;
                return;
            }

            _playerSkillPointsText.text = playerPoints.ToString();
            _requiredSkillPointsText.text = $"{requiredPoints}";

            if (playerPoints < requiredPoints)
            {
                _requiredSkillPointsText.color = _unavailableCellColor;
            }
            else
            {
                _requiredSkillPointsText.color = Color.white;
            }
        }

        public void UpdateSkillState(CellState state)
        {
            _skillStateText.text = state.ToString();

            // coloring
            if (state == CellState.None)
            {
                _skillStateText.text = string.Empty;
                return;
            }

            switch (state)
            {
                case CellState.Unavailable:
                    _skillStateIconImage.sprite = _unavailableIconSprite;
                    _skillStateIconImage.color = _unavailableCellColor;
                    break;
                case CellState.Locked:
                    _skillStateIconImage.sprite = _lockedIconSprite;
                    _skillStateIconImage.color = _lockedCellColor;
                    break;
                case CellState.Unlocked:
                    _skillStateIconImage.sprite = _unlockedIconSprite;
                    _skillStateIconImage.color = _unlockedCellColor;
                    break;
            }
        }
    }
}
