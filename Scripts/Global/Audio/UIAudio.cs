using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Sound
{
    public class UIAudio : MonoBehaviour
    {
        [field: SerializeField, Header("References")] public Audio NavigateMenuAudio { get; private set; }
    }
}
