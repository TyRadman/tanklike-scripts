using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    public class BossKey_GateSnap : MonoBehaviour
    {
        [SerializeField] private Animation _animation;

        public void PlayAnimation()
        {
            _animation.Play();
        }
    }
}
