using UnityEngine;

namespace TankLike.Attributes
{
    /// <summary>
    /// Attribute to get a reference to a component in the GameObject itself.
    /// </summary>
    public class InSelf : PropertyAttribute
    {
        /// <summary>
        /// If true, the attribute will add the component to the GameObject if it doesn't exist.
        /// </summary>
        public bool ForceReference { get; }

        public InSelf(bool forceReference = false)
        {
            ForceReference = forceReference;
        }
    }
}
