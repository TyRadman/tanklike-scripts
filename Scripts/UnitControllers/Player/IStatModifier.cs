using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IStatModifier
    {
        void AddStatModifier(IStatModifiersController controller);
        void RemoveStatModifier(IStatModifiersController controller);
    }
}
