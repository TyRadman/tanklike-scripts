using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IVisitor 
    {
        void Visit<IVisitable>();
    }
}
