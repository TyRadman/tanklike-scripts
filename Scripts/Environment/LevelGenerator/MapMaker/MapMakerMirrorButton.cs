using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerMirrorButton : MonoBehaviour
    {
        [SerializeField] private MirrorType _mirroringStyle;
        [SerializeField] private Image _bgImage;
        
        private MapMakerMirroringController _controller;

        public void SetUp(MapMakerMirroringController controller)
        {
            _controller = controller;

            _bgImage.color = Colors.DarkGray;
        }

        public void OnClicked()
        {
            _controller.DeselectAllButtons();
            _controller.SetMirrorType(_mirroringStyle);
            _bgImage.color = Colors.DarkRed;
        }

        public void Deselect()
        {
            _bgImage.color = Colors.DarkGray;
        }
    }
}
