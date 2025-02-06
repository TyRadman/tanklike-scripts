using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Elements;

namespace TankLike
{
    public interface IElementTarget
    {
        public void TakeElementEffect(ElementEffect element);
    }
}
