using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    [CreateAssetMenu(fileName = "PD_NAME", menuName = Directories.PLAYERS + "Player Data")]
    public class PlayerData : UnitData
    {
        [field: SerializeField] public string PlayerName { get; private set; }
        [field: SerializeField] public PlayerType PlayerType { get; private set; }
        [field: SerializeField] public PlayerComponents Prefab { get; private set; }
        [field: SerializeField] public UnitParts PartsPrefab { get; private set; }
        [field: SerializeField] public PlayerSkin[] Skins { get; private set; }
        [field: SerializeField] public Sprite PlayerIcon { get; private set; }

        [field: SerializeField] public float MovementSpeed { get; set; }
        [field: SerializeField] public int MaxHealthPoints { get; set; }
        [field: SerializeField] public float MaxFuel { get; set; }
        [field: SerializeField] public float MaxEnergyPoints { get; set; }

        internal Sprite GetIcon()
        {
            return PlayerIcon;
        }

        internal Color GetSkinColor(int index)
        {
            return Skins[index].Color;
        }
    }
}
