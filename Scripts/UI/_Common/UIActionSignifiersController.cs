using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.UI.Signifiers
{
    public class UIActionSignifiersController : MonoBehaviour, ISignifierController
    {
        [SerializeField] private List<UIActionSignifier> _actionSignifiers = new List<UIActionSignifier>();
        [SerializeField] private HorizontalLayoutGroup _layoutGroup; 
        private UIActionSignifier _lastSetSignifier;

        public void SetUp()
        {
            _actionSignifiers.ForEach(a => a.Disable());
        }

        public void DisplaySignifier(string action, string key)
        {
            _lastSetSignifier = _actionSignifiers.Find(a => !a.IsActive);

            if(_lastSetSignifier == null)
            {
                Debug.LogError($"All action signifiers are active.");
                return;
            }

            _lastSetSignifier.SetText(action, key);
        }

        public void SetLastSignifierAsParent()
        {
            _lastSetSignifier.IsParentSignifier = true;
        }

        public void ClearChildSignifiers()
        {
            _actionSignifiers.FindAll(a => !a.IsParentSignifier).ForEach(a => a.Disable());
        }

        public void ClearParentSignifiers()
        {
            _actionSignifiers.FindAll(a => a.IsParentSignifier).ForEach(a => a.Disable());
        }

        public void ClearAllSignifiers()
        {
            _actionSignifiers.ForEach(a => a.Disable());
        }
    }
}
