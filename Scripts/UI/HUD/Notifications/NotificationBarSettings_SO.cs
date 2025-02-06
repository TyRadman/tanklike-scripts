using UnityEngine;

namespace TankLike.UI.Notifications
{
    [CreateAssetMenu(fileName = "Notification Settings", menuName = Directories.UI + "Notification Settings")]
    public class NotificationBarSettings_SO : ScriptableObject
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Color AmountColor { get; private set; }
    }
}
