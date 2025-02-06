using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cheats
{
    [CreateAssetMenu(fileName = "CheatParent", menuName = Cheat.ROOT + "Parent/CheatParent")]
    public class CheatsParent : CheatCell
    {
        public CheatsParent PreviousParent { get; set; }
        public List<CheatCell> Cheats;
    }
}
