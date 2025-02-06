using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    using UI.Notifications;

    public class CoinCollectable : Collectable
    {
        [SerializeField] private int _amount;
        [SerializeField] private Animation _animation;
        private const float MAX_ANIMATION_START_DELAY = 0.5f;

        public override void OnCollected(IPlayerController player)
        {
            NotificationBarSettings_SO settings = GameManager.Instance.Constants.CoinsNotificationSettings;

            if(player == null)
            {
                Debug.LogError("player is null");
                return;
            }

            GameManager.Instance.NotificationsManager.PushCollectionNotification(settings, _amount, player.PlayerIndex);

            GameManager.Instance.PlayersManager.Coins.AddCoins(_amount);
            base.OnCollected(player);
        }

        protected override void OnBounceFinished()
        {
            base.OnBounceFinished();

            if(_animation != null)
            {
                Invoke(nameof(PlayAnimation), Random.Range(0f, MAX_ANIMATION_START_DELAY));
            }
        }

        private void PlayAnimation()
        {
            _animation.Play();
        }
    }
}
