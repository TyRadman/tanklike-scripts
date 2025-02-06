using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class StartFoldAttribute : PropertyAttribute
    {
        public string Title;

        public StartFoldAttribute(string title)
        {
            Title = title;
        }
    }

    public class EndFoldAttribute : PropertyAttribute
    {

    }
}
