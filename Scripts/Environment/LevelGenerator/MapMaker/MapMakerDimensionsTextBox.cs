using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TankLike.Environment.MapMaker
{
    /// <summary>
    /// The text box that contains and is used to update the level dimensions.
    /// </summary>
    public class MapMakerDimensionsTextBox : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private bool _isXAxis = true;
        private MapMakerSelector _selector;

        public void SetUp(MapMakerSelector selector)
        {
            _selector = selector;

            // update the input field's values
            int value = _isXAxis ? _selector.LevelDimensions.x : _selector.LevelDimensions.y;
            _inputField.text = value.ToString();
        }

        /// <summary>
        /// Added as a listener to the input field's onEditEnded
        /// </summary>
        /// <param name="value"></param>
        public void OnTextFieldValueChanged(string value)
        {
            if (int.TryParse(value, out int result))
            {
                Vector2Int dimensions = _selector.LevelDimensions;

                if (_isXAxis)
                {
                    dimensions.x = result;
                }
                else
                {
                    dimensions.y = result;
                }

                _selector.UpdateLevelDimension(dimensions);
            }
        }
    }
}
