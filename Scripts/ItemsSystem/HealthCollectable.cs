using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    using UnitControllers;
    using Misc;

    public class HealthCollectable : Collectable
    {
        [SerializeField] private int _energyAmount = 10;

        public override void OnCollected(IPlayerController player)
        {
            GameManager.Instance.NotificationsManager.PushCollectionNotification(_notificationSettings, _energyAmount, player.PlayerIndex);

            base.OnCollected(player);

            if (player is not PlayerComponents playerComponents)
            {
                Debug.LogError("collector is not PlayerComponents, therefore, no energy is going to be added.");
                return;
            }

            playerComponents.Energy.AddEnergy(_energyAmount);
        }

        protected override ParticleSystemHandler GetPoofParticles()
        {
            return GameManager.Instance.VisualEffectsManager.Misc.OnEnergyCollectedPoof;
        }
    }
}
