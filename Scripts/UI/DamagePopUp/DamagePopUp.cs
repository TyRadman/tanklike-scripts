using UnityEngine;
using TMPro;
using TankLike.Utils;

namespace TankLike.UI.DamagePopUp
{
    [SelectionBase]
    public class DamagePopUp : MonoBehaviour, IPoolable
    {
        public System.Action<IPoolable> OnReleaseToPool { get; private set; }
        
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private Animation _animation;
        [SerializeField] private MeshRenderer _mesh;

        private int _lastAmount = 0;

        public void SetUp(int amount, DamagePopUpInfo info, float animationSpeed = 1f)
        {
            _text.enabled = true;
            _mesh.enabled = true;

            // if there is a change to the animation speed then apply the change
            if (animationSpeed != 1f)
            {
                _animation[_animation.clip.name].speed = animationSpeed;
            }

            _text.text = $"{info.Prefix} {amount} {info.Suffix}";
            _text.color = info.TextColor;

            // resize the font based on the amount passed
            if (amount == _lastAmount)
            {
                _text.fontSize = info.FontSizeRange.Lerp(Mathf.InverseLerp(info.AmountRange.x, info.AmountRange.y, amount));
            }
            
            // cache the last amount to avoid setting the size again
            _lastAmount = amount;
            _animation.Play();
        }

        #region Pool
        public void Init(System.Action<IPoolable> OnRelease)
        {
            OnReleaseToPool = OnRelease;
        }

        public void OnRelease()
        {
            _text.enabled = false;
            _mesh.enabled = false;
        }

        public void OnRequest()
        {

        }

        public void Clear()
        {
            Destroy(gameObject);
        }

        // it's called in the animation
        public void TurnOff()
        {
            OnReleaseToPool?.Invoke(this);
        }
        #endregion
    }
}
