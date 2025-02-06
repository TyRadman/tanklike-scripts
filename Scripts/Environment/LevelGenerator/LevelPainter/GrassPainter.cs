using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Utils;

    public class GrassPainter : RoomPainter
    {
        [System.Serializable]
        public struct GrassColors
        {
            public Color FrontTopColor;
            public Color FrontBottomColor;
            public Color BackTopColor;
            public Color BackBottomColor;
        }

        [System.Serializable]
        public struct GrassTexture
        {
            public Texture2D Texture;
            public float Probability;
            [Range(0f, 1f)] public float ColorInterpolationStartPoint;
        }

        [SerializeField] private GrassProp _grassPrefab;
        [SerializeField] private List<GrassTexture> _grassTexturesChances = new List<GrassTexture>();
        [SerializeField] private List<GrassColors> _grassColors = new List<GrassColors>();

        public override void PaintRoom(MapTiles_SO map, Room room)
        {
            List<Tile> tiles = map.Tiles;
            List<Tile> groundTiles = GetTilesByRules(tiles, PaintingRules);
            //List<Tile> groundTiles = tiles.FindAll(t => t.Tag == TileTag.Ground &&
            //!HasNeighborWithinDepth(t, TileType.Wall, 2, tiles));

            groundTiles.RemoveAll(t => t.BuiltTile == null || t == null);

            if (groundTiles.Count == 0)
            {
                Debug.Log("No suitable tiles found for grass");
                return;
            }

            int grassCount = _levelData.GrassRangePerRoom.RandomValue();

            for (int i = 0; i < grassCount; i++)
            {
                Tile selectedTile = groundTiles.RandomItem();

                Vector3 offset;

                // ensure the grass spawns between tiles
                if (Random.value >= 0.5f)
                {
                    offset = new Vector3(Constants.TILE_SIZE / 2f * (Random.value >= 0.5f? 1f : -1f), 0f, Constants.TILE_SIZE.GetRandomValue() * 0.5f);
                }
                else
                {
                    offset = new Vector3(Constants.TILE_SIZE.GetRandomValue() * 0.5f, 0f, Constants.TILE_SIZE / 2f * (Random.value >= 0.5f ? 1f : -1f));
                }

                Vector3 position = selectedTile.BuiltTile.transform.position + offset;

                GrassProp grass = Instantiate(_grassPrefab, position, Quaternion.identity, room.Spawnables.SpawnablesParent);
                grass.transform.eulerAngles = new Vector3().Add(y: Random.Range(0, 360));


                Dictionary<System.Func<GrassTexture>, float> textureChances = new Dictionary<System.Func<GrassTexture>, float>();

                foreach (GrassTexture grassTexture in _grassTexturesChances)
                {
                    textureChances.Add(() => grassTexture, grassTexture.Probability);
                }

                GrassTexture backwardSelectedTexture = ChanceSelector.SelectByChance(textureChances);
                GrassTexture forwardSelectedTexture = ChanceSelector.SelectByChance(textureChances);

                grass.ApplyTexture(backwardSelectedTexture.Texture, forwardSelectedTexture.Texture);
                grass.SetStartColorThreshold(backwardSelectedTexture.ColorInterpolationStartPoint);
                grass.ApplyColors(_grassColors.RandomItem());
            }
        }
    }
}
