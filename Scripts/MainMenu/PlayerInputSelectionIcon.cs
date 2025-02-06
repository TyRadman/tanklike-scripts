using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public class PlayerInputSelectionIcon : MonoBehaviour
    {
        public GameObject ControllerIcon;
        public GameObject KeyboardIcon;
        public GameObject ConfirmText;
        public Animation JoinText;

        private void Awake()
        {
            ControllerIcon.SetActive(false);
            KeyboardIcon.SetActive(false);
            ConfirmText.SetActive(false);
            JoinText.gameObject.SetActive(true);
        }
    }
}
