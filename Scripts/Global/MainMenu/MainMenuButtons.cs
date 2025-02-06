using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike
{
    public class MainMenuButtons : MonoBehaviour
    {
        public int SceneToLoad = 1;
        [SerializeField] private GameObject _buttonsCanvas;

        public void OnPlayerRoomButton()
        {
            SceneToLoad = 1;
            _buttonsCanvas.SetActive(false);
            print("Clicked");
        }

        public void OnBossGateRoomButton()
        {
            SceneToLoad = 2;
            _buttonsCanvas.SetActive(false);
            print("Clicked");
        }

        public void ExitButton()
        {
            Application.Quit();
        }
    }
}
