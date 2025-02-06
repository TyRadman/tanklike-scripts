using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public abstract class UICell : MonoBehaviour
    {
        public abstract void SetUp();
        public abstract void Highlight();
        public abstract void Unhighlight();
        public abstract void SetIcon(Sprite iconSprite);
    }
}
