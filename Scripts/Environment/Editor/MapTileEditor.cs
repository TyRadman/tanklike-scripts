using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TankLike.Combat.Destructible;
using TankLike.EditorStuff;

namespace TankLike.Environment.MapMaker
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MapTiles_SO))]
    public class MapTileEditor : UnityEditor.Editor
    {
        private MapTiles_SO Map;
        private const int TILE_PIXEL_SIZE = 5;
        private bool ShowGrid = false;
        private bool foldOut = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Map = (MapTiles_SO)target;

            if (GUILayout.Button("Cache Neighbours"))
            {
                Map.CacheSurroundingTilesIndices();
            }

            GUILayout.Space(21f);
            DrawTileTexture(Map.Tiles);
            GUILayout.Space(5f);

            foldOut = EditorGUILayout.BeginFoldoutHeaderGroup(foldOut, "Settings");

            if (foldOut)
            {
                EditorGUI.indentLevel++;

                ShowGrid = GUILayout.Toggle(ShowGrid, "Show Grid");
                MapEditorDisplayColors.WALL_COLOR = EditorGUILayout.ColorField("Wall Color", MapEditorDisplayColors.WALL_COLOR);
                MapEditorDisplayColors.GROUND_COLOR = EditorGUILayout.ColorField("Ground Color", MapEditorDisplayColors.GROUND_COLOR);
                MapEditorDisplayColors.GATE_COLOR = EditorGUILayout.ColorField("Gate Color", MapEditorDisplayColors.GATE_COLOR);
                MapEditorDisplayColors.OVERLAY_CRATE_COLOR = EditorGUILayout.ColorField("Destructible Color", MapEditorDisplayColors.OVERLAY_CRATE_COLOR);
                MapEditorDisplayColors.SPAWN_POINTS_COLOR = EditorGUILayout.ColorField("Spawn point Color", MapEditorDisplayColors.SPAWN_POINTS_COLOR);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawTileTexture(List<Tile> tiles)
        {
            if (tiles.Count == 0)
            {
                Debug.Log("Map has zero tiles");
                return;
            }

            int textureWidth = (tiles.Max(t => t.Dimension.x) + 1) * TILE_PIXEL_SIZE;
            int textureHeight = (tiles.Max(t => t.Dimension.y) + 1) * TILE_PIXEL_SIZE;

            Texture2D texture = new Texture2D(textureWidth, textureHeight);

            float aspectRatio = (float)textureWidth / textureHeight;

            // cache the maxSize which should be the width since the height is infinite
            float maxSize = EditorGUIUtility.currentViewWidth;
            // resize based on ratio
            float scaledWidth = Mathf.Clamp(maxSize, 1f, maxSize * aspectRatio);
            float scaledHeight = Mathf.Clamp(maxSize / aspectRatio, 1f, maxSize);
            // set the colors in accordance to the tiles types
            Color[] clearColorArray = Enumerable.Repeat(MapEditorDisplayColors.GROUND_COLOR, textureWidth * textureHeight).ToArray();
            texture.SetPixels(clearColorArray);

            foreach (var tile in tiles)
            {
                // if the tile we're about to draw is a ground tile (default color) and it doesn't have an overlay, then skip
                if ((int)tile.Tag >= 0 && (int)tile.Tag <= 3 && tile.Overlays.Count == 0 && !ShowGrid) continue;

                int x = Mathf.RoundToInt(tile.Dimension.x) * TILE_PIXEL_SIZE;
                int y = Mathf.RoundToInt(tile.Dimension.y) * TILE_PIXEL_SIZE;

                Color color = GetColorBasedOnTileType(tile);

                for (int i = 0; i < TILE_PIXEL_SIZE; i++)
                {
                    for (int j = 0; j < TILE_PIXEL_SIZE; j++)
                    {
                        if (i == 0 || i == TILE_PIXEL_SIZE - 1 || j == 0 || j == TILE_PIXEL_SIZE - 1)
                        {
                            texture.SetPixel(x + i, y + j, Color.black);
                            continue;
                        }

                        texture.SetPixel(x + i, y + j, color);
                    }
                }
            }

            texture.Apply();
            Rect textureRect = GUILayoutUtility.GetRect(scaledWidth, scaledHeight, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.DrawPreviewTexture(textureRect, texture);
        }

        private Color GetColorBasedOnTileType(Tile tile)
        {
            if (tile.Overlays.Count > 0)
            {
                if (tile.Overlays[0] == DestructableTag.SpawnPoint)
                {
                    return MapEditorDisplayColors.SPAWN_POINTS_COLOR;
                }
                else if (tile.Overlays[0] == DestructableTag.BossSpawnPoint)
                {
                    return MapEditorDisplayColors.BOSS_SPAWN_POINTS_COLOR;
                }

                return MapEditorDisplayColors.OVERLAY_CRATE_COLOR;
            }

            int tag = (int)tile.Tag;

            if (tag >= 0 && tag <= 3)
            {
                return MapEditorDisplayColors.GROUND_COLOR;
            }
            else if (tag >= 4 && tag <= 9)
            {
                return MapEditorDisplayColors.WALL_COLOR;
            }
            else
            {
                return MapEditorDisplayColors.GATE_COLOR;
            }
        }
    }
}
