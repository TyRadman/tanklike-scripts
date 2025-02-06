using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TankLike.Tutorial;
using TankLike.Combat;

namespace TankLike
{
    public class TutorialBoostTimer : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;
        [SerializeField] private float time = 10f;
        [SerializeField] private TutorialBoostTimerStartPoint _starter;
        [SerializeField] private TutorialManager _tutorialManager;

        public void SetUp()
        {
            GameManager.Instance.CameraManager.Zoom.SetToTutorialBoostZoom();
            _tutorialManager.OnPlayerDeath += OnPlayerDeath;
        }

        public void StartTimer()
        {
            StartCoroutine(CountDownRoutine());
        }

        private IEnumerator CountDownRoutine()
        {
            float timeElapsed = time;

            while(timeElapsed > 0)
            {
                timeElapsed -= Time.deltaTime;

                _text.text = $"{timeElapsed:0.0}";

                yield return null;
            }

            DamageInfo damageInfo = DamageInfo.Create()
                .SetDamage(10000)
                .Build();

            GameManager.Instance.PlayersManager.GetPlayer(0).Health.TakeDamage(damageInfo);
        }

        private void OnPlayerDeath()
        {
            _starter.gameObject.SetActive(true);
        }

        public void OnOver()
        {
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();
            StopAllCoroutines();
            _starter.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
