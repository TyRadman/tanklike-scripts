using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Cam
{
    [System.Serializable]
    public class CameraLimits
    {
        public Vector2 HorizontalLimits;
        public Vector2 VerticalLimits;

        /// <summary>
        /// Adds the limit values to this set of limit values.
        /// </summary>
        /// <param name="limits">Limits to add the values of.</param>
        public void AddOffset(Vector3 offset)
        {
            HorizontalLimits.x += offset.x;
            HorizontalLimits.y += offset.x;
            VerticalLimits.x += offset.z;
            VerticalLimits.y += offset.z;
        }

        /// <summary>
        /// Set this set of limit values to the provided limits.
        /// </summary>
        /// <param name="limits">Limit values to set to.</param>
        public void SetValues(CameraLimits limits)
        {
            HorizontalLimits = limits.HorizontalLimits;
            VerticalLimits = limits.VerticalLimits;
        }

        /// <summary>
        /// Sets new values to the camera limits with an offset and a multiplier to the offset so that the offset scales with different zoom levels.
        /// </summary>
        /// <param name="originalLimits">Limit values to set this limits to.</param>
        /// <param name="offsetLimits">Limit values that act as offset values.</param>
        /// <param name="multiplier">The multiplier by which the offset values are multiplied.</param>
        public void SetValuesWithOffset(CameraLimits originalLimits, CameraLimits offsetLimits, float multiplier = 1f)
        {
            // set the original limts, the ones of the room's position
            HorizontalLimits = originalLimits.HorizontalLimits;
            VerticalLimits = originalLimits.VerticalLimits;

            // add the offset and apply the multiplier to them. The higher the multiplier value, the smaller the offset should get
            HorizontalLimits.x += offsetLimits.HorizontalLimits.x * multiplier;
            HorizontalLimits.y -= offsetLimits.HorizontalLimits.y * multiplier;
            VerticalLimits.x += offsetLimits.VerticalLimits.x * multiplier;
            VerticalLimits.y -= offsetLimits.VerticalLimits.y * multiplier;

            if(HorizontalLimits.x > HorizontalLimits.y)
            {
                float middlePoint = (HorizontalLimits.x + HorizontalLimits.y) / 2f;
                HorizontalLimits = Vector2.one * middlePoint;
            }

            if (VerticalLimits.x > VerticalLimits.y)
            {
                float middlePoint = (VerticalLimits.x + VerticalLimits.y) / 2f;
                VerticalLimits = Vector2.one * middlePoint;
            }
        }


        #region Debug
        public void PrintValues(string name = "")
        {
            Debug.Log($"{name}\nHorizontal Limits: {HorizontalLimits}\nVertical Limits:{VerticalLimits}");
        }
        #endregion
    }
}
