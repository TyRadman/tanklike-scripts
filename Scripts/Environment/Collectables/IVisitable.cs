using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IVisitable 
    {
        void Accept(IVisitor visitor);
    }
}
