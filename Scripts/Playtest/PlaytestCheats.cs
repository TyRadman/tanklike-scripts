using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankLike.Playtest
{
    public class PlaytestCheats : MonoBehaviour
    {
        private bool _forceRestart;

        private void Start()
        {
            _forceRestart = false;
        }

        private void Update()
        {
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.R) && !_forceRestart)
            {
                _forceRestart = true;
                ForceRestart();
            }
        }

        private void ForceRestart()
        {
            Debug.Log("Called");
            GameManager.Instance.DisposeCurrentSceneController();
            GameManager.Instance.SceneLoadingManager.SwitchScene(SceneManager.GetActiveScene().name, Scenes.GAMEPLAY, true);
        }
    }
}
