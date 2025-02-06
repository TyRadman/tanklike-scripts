using UnityEngine;

namespace TankLike.Attributes
{
    /// <summary>
    /// Attribute to get a reference to a component in the children of the GameObject.
    /// </summary>
    public class InChildren : PropertyAttribute
    {
        /// <summary>
        /// If true, the attribute will add a child gameobject with the required component if it doesn't exist.
        /// </summary>
        public bool ForceReference { get; }

        public InChildren(bool forceReference = false)
        {
            ForceReference = forceReference;
        }
    }
}
