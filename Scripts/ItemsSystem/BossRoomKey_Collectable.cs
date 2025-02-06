using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.ItemsSystem
{
    using UnitControllers;

    public class BossRoomKey_Collectable : Collectable
    {
        public override void OnCollected(IPlayerController player)
        {
            base.OnCollected(player);

            // increment the count
            GameManager.Instance.BossKeysManager.OnKeyCollected();
        }

        public override void DisableCollectable()
        {
            base.DisableCollectable();

            gameObject.SetActive(false);
        }
    }
}
