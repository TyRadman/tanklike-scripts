using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    /// <summary>
    /// An interface for components that have the ability to resume their activity. 
    /// </summary>
    public interface IResumable
    {
        /// <summary>
        /// Resumes the activity of the component based on whether the input of the activity is being performed.
        /// </summary>
        void ResumeComponent();
    }
}
