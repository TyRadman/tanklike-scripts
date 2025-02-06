using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TankLike.Combat.SkillTree
{
    public class SingleSkillSelectionSection : MonoBehaviour, ICellSelectable
    {
        [SerializeField] private TextMeshProUGUI _skillNameText;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private List<Connection> _connections;
        [Header("Navigation text")]
        [SerializeField] private TextMeshProUGUI _leftButtonText;
        [SerializeField] private TextMeshProUGUI _rightButtonText;
        [SerializeField, TextArea(0, 5)] private string _skillCategoryDescription;

        [Header("Events")]
        [SerializeField] private UnityEvent _onHighlightAction;
        [SerializeField] private UnityEvent _onUnhighlightAction;


        private List<SkillProfile> _skillProfiles = new List<SkillProfile>();
        private SkillProfile _selectedSkill;
        private int _currentSkillIndex = 0;

        public void Initiate()
        {
            Unhighlight();
            _currentSkillIndex = 0;

            _selectedSkill = _skillProfiles[_currentSkillIndex];
            SetSkillDetails(_selectedSkill);
        }

        public void AddSkillProfile(SkillProfile profile)
        {
            _skillProfiles.Add(profile);
        }

        public void Highlight()
        {
            _onHighlightAction?.Invoke();
        }

        public void Unhighlight()
        {
            _onUnhighlightAction?.Invoke();
        }

        public void NavigateHorizontally(Direction direction)
        {
            _currentSkillIndex += direction == Direction.Left ? -1 : direction == Direction.Right? 1 : 0;

            if (_currentSkillIndex < 0)
            {
                _currentSkillIndex = _skillProfiles.Count - 1;
            }
            else if (_currentSkillIndex >= _skillProfiles.Count)
            {
                _currentSkillIndex = 0;
            }

            _selectedSkill = _skillProfiles[_currentSkillIndex];
            SetSkillDetails(_selectedSkill);
        }

        public void SetSkillDetails(SkillProfile profile)
        {
            _skillNameText.text = profile.SkillHolder.GetName();
            _skillIconImage.sprite = profile.SkillHolder.GetIcon();
        }

        public ICellSelectable Navigate(Direction direction)
        {
            MonoBehaviour target = _connections.Find(c => c.Direction == direction).Target;

            if (target is not null and ICellSelectable selectable)
            {
                return selectable;
            }

            return null;
        }

        public SkillProfile GetActiveSkill()
        {
            return _selectedSkill;
        }

        public void SetLeftArrowInputText(string text)
        {
            _leftButtonText.text = text;
        }

        public void SetRightArrowInputText(string text)
        {
            _rightButtonText.text = text;
        }

        internal string GetSkillCategoryDescription()
        {
            return _skillCategoryDescription;
        }
    }
}
