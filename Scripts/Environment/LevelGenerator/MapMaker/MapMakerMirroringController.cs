using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.MapMaker
{
    public class MapMakerMirroringController : MonoBehaviour
    {
        [SerializeField] private List<MapMakerMirrorButton> _buttons = new List<MapMakerMirrorButton>();
        [SerializeField] private MapMakerSelector _selector;
        private MirrorType _mirrorType;

        public void SetUp()
        {
            if(_buttons.Count == 0)
            {
                return;
            }

            _buttons.ForEach(b => b.SetUp(this));
            _buttons[0].OnClicked();
        }

        public void DeselectAllButtons()
        {
            _buttons.ForEach(b => b.Deselect());
        }

        public void BuildTile(int x, int y)
        {
            switch (_mirrorType)
            {
                case MirrorType.None:
                    BuildOneTile(x, y);
                    break;
                case MirrorType.MirrorX:
                    BuildXMirrorTile(x, y);
                    break;
                case MirrorType.MirrorY:
                    BuildYMirrorTile(x, y);
                    break;
                case MirrorType.MirrorXY:
                    BuildXYMirrorTile(x, y);
                    break;
                case MirrorType.Mirror4D:
                    Build4DMirrorTile(x, y);
                    break;
            }
        }

        internal void SetMirrorType(MirrorType mirroringStyle)
        {
            _mirrorType = mirroringStyle;
        }

        private void BuildOneTile(int x, int y)
        {
            _selector.BuildTileAtAxis(x, y);
        }

        private void BuildXMirrorTile(int x, int y)
        {
            int mirrorX = _selector.LevelDimensions.x - 1 - x;

            _selector.BuildTileAtAxis(x, y);
            _selector.BuildTileAtAxis(mirrorX, y);
        }

        private void BuildYMirrorTile(int x, int y)
        {
            int mirrorY = _selector.LevelDimensions.y - 1 - y;

            _selector.BuildTileAtAxis(x, y);
            _selector.BuildTileAtAxis(x, mirrorY);
        }

        private void BuildXYMirrorTile(int x, int y)
        {
            int mirrorX = _selector.LevelDimensions.x - 1 - x;
            int mirrorY = _selector.LevelDimensions.y - 1 - y;

            _selector.BuildTileAtAxis(x, y);
            _selector.BuildTileAtAxis(mirrorX, mirrorY);
        }

        private void Build4DMirrorTile(int x, int y)
        {
            int mirrorX = _selector.LevelDimensions.x - 1 - x;
            int mirrorY = _selector.LevelDimensions.y - 1 - y;

            _selector.BuildTileAtAxis(x, y);
            _selector.BuildTileAtAxis(x, mirrorY);
            _selector.BuildTileAtAxis(mirrorX, y);
            _selector.BuildTileAtAxis(mirrorX, mirrorY);
        }
    }

    public enum MirrorType
    {
        None = 0, MirrorX = 1, MirrorY = 2, MirrorXY = 3, Mirror4D = 4
    }
}
