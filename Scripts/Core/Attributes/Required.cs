using System;
using UnityEngine;

namespace TankLike.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Required : PropertyAttribute { }
}
