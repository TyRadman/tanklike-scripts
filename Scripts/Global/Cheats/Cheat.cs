using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TankLike.Cheats
{
    public abstract class Cheat : CheatCell
    {
        public const string ON = "On";
        public const string OFF = "Off";
        public const string ROOT = Directories.MAIN + "Cheats/";
        public const string NAME = "Cheat_";


    }
}
