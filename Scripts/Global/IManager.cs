using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public interface IManager
    {
        public bool IsActive { get; }

        public void SetUp();
        public void Dispose();
    }
}
