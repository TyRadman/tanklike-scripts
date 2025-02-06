using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankLike.Utils;
using TankLike.Combat.Destructible;

namespace TankLike.Environment.MapMaker
{
    [CreateAssetMenu(fileName = "Map Styler", menuName = "Level/ Map Styler")]
    public class MapTileStyler : ScriptableObject
    {
        [System.Serializable]
        public class TilesGroup
        {
            public TileTag Tag;
            public List<GameObject> Tiles;
            public List<Material> Materials;
            public List<Mesh> Meshes;
        }

        [System.Serializable]
        public class GroundTilesGroup
        {
            public TileTag Tag;
            public List<Material> Materials;
            public List<Mesh> Meshes;
        }

        [System.Serializable]
        public class WallTilesGroup
        {
            public TileTag Tag;
            public List<WallTile> Tiles;
        }

        [System.Serializable]
        public class OverLays
        {
            public DestructableTag Tag;
            public GameObject OverlayObject;
        }

        [Header("Ground Tiles")]
        [SerializeField] private GroundTile _groundPrefab;
        [SerializeField] List<GroundTilesGroup> _groundTiles;
        [field: SerializeField] public float RandomHorizontalRotationChance { get; private set; }
        [field: SerializeField] public Vector2 RandomHorizontalRotationRange { get; private set; }
        [field: SerializeField] public float RandomVerticalRotationChance { get; private set; }
        [field: SerializeField] public Vector2 RandomVerticalRotationRange { get; private set; }
        [field: SerializeField] public float RandomVerticalOffsetChance { get; private set; }
        [field: SerializeField] public Vector2 RandomVerticalOffsetRange { get; private set; }

        [Header("Wall Tiles")]
        [SerializeField] List<WallTilesGroup> _wallTiles;

        [Header("Destructable Walls")]
        [SerializeField] private DestructibleWall _destructibleWallPrefab;
        [field: SerializeField] public float DestructibleWallRandomRotationChance { get; private set; }
        [field: SerializeField] public Vector2 DestructibleWallRandomRotationRange { get; private set; }
        [field: SerializeField] public float DestructibleWallRandomScaleChance { get; private set; }
        [field: SerializeField] public Vector2 DestructibleWallRandomScaleRange { get; private set; }


        public List<TilesGroup> Tiles;

        public List<OverLays> OverlayReferences; 

        public GameObject GetRandomTileByTag(TileTag tag)
        {
            if (Tiles.Find(t => t.Tag == tag) == null || Tiles.Find(t => t.Tag == tag).Tiles.Count == 0)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            return Tiles.Find(t => t.Tag == tag).Tiles.RandomItem();
        }

        public GameObject GetGroundTile(TileTag tag)
        {
            if (_groundTiles.Find(t => t.Tag == tag) == null)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            return _groundPrefab.gameObject;
        }

        public void RandomizeGroundTileVisuals(TileTag tag, ref GroundTile tile)
        {
            if (_groundTiles.Find(t => t.Tag == tag) == null)
            {
                //Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return;
            }

            GroundTilesGroup tiles = _groundTiles.Find(t => t.Tag == tag);

            // Set a random mesh
            tile.MeshFilter.mesh = tiles.Meshes.RandomItem();

            // Set a random material
            tile.MeshRenderer.material = tiles.Materials.RandomItem();
        }

        public GameObject GetRandomWallTile(TileTag tag)
        {
            if (_wallTiles.Find(t => t.Tag == tag) == null)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            WallTile tile =  _wallTiles.Find(t => t.Tag == tag).Tiles.RandomItem();

            return tile.gameObject;
        }

        public GameObject GetTile(TileTag tag)
        {
            if (Tiles.Find(t => t.Tag == tag) == null || Tiles.Find(t => t.Tag == tag).Tiles.Count == 0)
            {
                Debug.LogError($"No tiles of type: {tag} exist in styler: {name}");
                return null;
            }

            return Tiles.Find(t => t.Tag == tag).Tiles[0];
        }

        public GameObject GetOverlay(DestructableTag tag)
        {
            if(!OverlayReferences.Exists(o => o.Tag == tag))
            {
                return null;
            }

            return OverlayReferences.Find(o => o.Tag == tag).OverlayObject;
        }

        public DestructibleWall GetDestructibleWall()
        {
            return _destructibleWallPrefab;
        }
    }
}
