using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class TutorialBoostTimerStartPoint : MonoBehaviour
    {
        [SerializeField] private TutorialBoostTimer _timer;

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _timer.StartTimer();
                gameObject.SetActive(false);
            }
        }
    }
}
