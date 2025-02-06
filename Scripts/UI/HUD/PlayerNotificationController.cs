using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.Notifications
{
    public class PlayerNotificationController : MonoBehaviour
    {
        [SerializeField] private List<CollectionNotificationBar> _bars;

        public void SetUp()
        {
            _bars.ForEach(bb => bb.SetUp());
        }

        public void Dispose()
        {
            _bars.ForEach(bb => bb.Dispose());
        }

        public void PushCollectionNotification(int amount, NotificationBarSettings_SO data)
        {
            if (data == null)
            {
                Debug.LogError("No settings passed");
                return;
            }

            CollectionNotificationBar selectedBar;

            // if there is a bar that holds the selected type of notification and it's not available i.e. is already displayed on the HUD
            if (_bars.Exists(b => b.Data == data && b.CanAddTo))
            {
                selectedBar = _bars.Find(b => b.Data == data && b.CanAddTo);
            }
            // otherwise, just get the next available bar
            else
            {
                selectedBar = _bars.Find(b => b.IsAvailable);
            }

            selectedBar.Display(data, amount);
        }
    }
}
