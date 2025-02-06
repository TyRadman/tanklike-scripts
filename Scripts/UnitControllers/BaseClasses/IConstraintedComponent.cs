using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.UnitControllers
{
    /// <summary>
    /// Interface for components that can be constrained by certain abilities.
    /// </summary>
    public interface IConstraintedComponent
    {
        /// <summary>
        /// Applies the specified constraints to the component.
        /// </summary>
        /// <param name="constraints">The constraints to apply.</param>
        void ApplyConstraint(AbilityConstraint constraints);

        /// <summary>
        /// Gets or sets a value indicating whether the component is constrained.
        /// </summary>
        bool IsConstrained { get; set; }
    }
}
