using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    [CreateAssetMenu(fileName = "Changing Speed", menuName = "Shot Configurations/Speed Over Lifetime/Changing Speed")]
    public class ChangingValueSpeedOverLife : SpeedOverLife
    {
        [Tooltip("How much to increment/decrement every second")]
        [SerializeField] private float _valueOverSeconds;
        [Tooltip("The value after which the speed changing stops")]
        [SerializeField] private float _maxAdditionalSpeed;
        private float _time = 0f;
        private bool _changeValue = true;

        public override float GetSpeed(float speed, float _deltaTime)
        {
            if (!_changeValue)
            {
                return speed + _maxAdditionalSpeed;
            }

            _time += _deltaTime;
            float additionalSpeed = _valueOverSeconds * _time;
            //Debug.Log($"Time:{_time}, speed: {speed + (_valueOverSeconds * _time)}");

            if (Mathf.Abs(additionalSpeed) <= Mathf.Abs(_maxAdditionalSpeed))
            {
                return speed + (_valueOverSeconds * _time);
            }
            else
            {
                _changeValue = false;
                return speed + _maxAdditionalSpeed;
            }

        }

        public override void Reset()
        {
            _time = 0f;
            _changeValue = true;
        }
    }
}
