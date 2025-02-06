using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    public class Shop_InteractableArea : InteractableArea
    {
        public override void Interact(int playerIndex)
        {
            base.Interact(playerIndex);

            GameManager.Instance.InteractableAreasManager.OpenShop(playerIndex);
        }
    }
}
