using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class HasInterface : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public HasInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException("Provided type is not an interface", nameof(interfaceType));
            }

            InterfaceType = interfaceType;
        }
    }
}
