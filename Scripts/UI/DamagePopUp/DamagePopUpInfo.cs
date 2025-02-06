using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UI.DamagePopUp
{
    [CreateAssetMenu(fileName = "PopUpInfo_NAME", menuName = Directories.COMBAT + "Damage Pop Up Info")]
    public class DamagePopUpInfo : ScriptableObject
    {
        public DamagePopUpType Type;
        public Vector2Int FontSizeRange;
        public Vector2Int AmountRange;
        public Color TextColor;
        public string Prefix;
        [field: SerializeField] public string Suffix { get; private set; }
    }
}
