using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = NAME + "SpawnWorkshop", menuName = ROOT + "Spawn Workshop")]
    public class Cheat_SpawnWorkshop : Cheat
    {
        public override void Initiate()
        {
            base.Initiate();
        }

        public override void PerformCheat()
        {
            GameManager.Instance.WorkshopController.SpawnWorkshopInRoom();
        }
    }
}
