using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [System.Serializable]
    public class PlayerSkin
    {
        [field: SerializeField] public Texture2D Texture { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
        [field: SerializeField] public Sprite Avatar { get; private set; }
    }
}
