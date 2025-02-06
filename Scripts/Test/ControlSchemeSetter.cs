using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike.Testing
{
    public class ControlSchemeSetter : MonoBehaviour
    {
        public const string CONTROL_SCHEME_KEY = "CS_P1";

        public void SelectScheme(int index)
        {
            PlayerPrefs.SetInt(CONTROL_SCHEME_KEY, index);
            SceneManager.LoadScene(1);
        }
    }
}
