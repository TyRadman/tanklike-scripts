using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    public static class Constants
    {
        public static int PlayerLayer { get; private set; } = 11;
        public static int EnemyLayer { get; private set; } = 10;
        public static int PlayerDamagableLayer { get; private set; } = 23;
        public static int EnemyDamagableLayer { get; private set; } = 24;
        public static float GroundHeight { get; private set; } = 0.1f;
        [Tooltip("The ratio of items' prices when they're sold vs bought.")]
        public const float SHOP_SELL_ITEMS_PRICE_MULTIPLIER = 0.25f;
        public const int REVIVAL_COST = 300;
        public static LayerMask MutualHittableLayer { get; private set; } = 1 << 12;
        public static LayerMask WallLayer { get; private set; } = 1 << 8;
        public static LayerMask GroundLayer { get; private set; } = 1 << 14;
        public static LayerMask DestructibleLayer { get; private set; } = 1 << 27;
        // tools
        public static int MaxToolsUsageCount { get; private set; } = 7;
        // UI
        public static float ButtonRepeatFrequency { get; private set; } = 0.1f;
        public static float PreButtonRepeatWaitTime { get; private set; } = 0.5f;
        public static float ShootingPointHeight { get; internal set; } = 1.1f;
        public static float MiniPlayerShootingPointHeight { get; internal set; } = 0.88f;
        public static float GroundtHeight { get; internal set; } = 0.08f;
        public static float OffsetToWalls { get; internal set; } = 0.5f;
        public static float AimAssistRayHeight { get; internal set; } = 0.2f;

        public const float TILE_SIZE = 2.0f;
    }

    public static class Directories
    {
        public const string MAIN = "TankLike/";
        public const string COMBAT = MAIN + "Combat/";
        public const string EDITOR = MAIN + "Editor/";
        public const string CAMERA = MAIN + "Camera/";
        public const string AMMUNITION = MAIN + "Ammunition/";
        public const string ABILITIES = MAIN + "Abilities/";
        public const string PLAYERS = MAIN + "Players/";
        public const string ENEMIES = MAIN + "Enemies/";
        public const string BOSSES = MAIN + "Bosses/";
        public const string SKILLS = MAIN + "Skills/";
        public const string TOOLS = MAIN + "Tools/";
        public const string UI = MAIN + "UI/";
        public const string POOLABLES = MAIN + "Poolables/";
        public const string ABILITIES_HOLDER = MAIN + "Abilities/Holders/";
        public const string SHOT_CONFIGURATIONS = AMMUNITION + "Shot Configurations/";
        public const string SKILL_TREE = PLAYERS + "Skill Tree/";
        public const string STATS_UPGRADES = SKILL_TREE + "Stat Upgrades/";
        public const string SPECIAL_UPGRADES = SKILL_TREE + "Special Upgrades/";
        public const string HOLDERS_UPGRADES = SKILL_TREE + "Holder Upgrades/";
        public const string TOOLS_SETTINGS = TOOLS + "Settings/";

        public const string DATA_COLLECTION = MAIN + "Data Collection/";
        public const string DATA = "Data/";
        public const string TUTORIAL = MAIN + "Tutorial/";
        public const string SETTINGS = MAIN + "Settings/";
        internal const string LEVEL = MAIN + "Level/";
        internal const string PRESETS = MAIN + "Presets/";
        internal const string OTHERS = MAIN + "Others/";
    }

    public static class Scenes
    {
        public const string BOOTSTRAP = "S_Bootstrap";
        public const string MAIN_MENU = "S_MainMenu";
        public const string ABILITY_SELECTION = "S_AbilitySelection";
        public const string LOBBY = "S_Lobby";
        public const string GAMEPLAY = "S_Gameplay";
        public const string BOSSES = "S_Bosses";
        public const string TUTORIAL = "S_Tutorial";
        public const string LOADING = "S_Loading";
        public const string PLAYGROUND = "S_Playground";
        public const string LEVEL_DESIGN = "S_LevelDesign";
    }

    public static class Colors
    {
        // Unity's Default Colors
        public static readonly Color Black = Color.black;
        public static readonly Color White = Color.white;
        public static readonly Color Red = Color.red;
        public static readonly Color Green = Color.green;
        public static readonly Color Blue = Color.blue;
        public static readonly Color Yellow = Color.yellow;
        public static readonly Color Cyan = Color.cyan;
        public static readonly Color Magenta = Color.magenta;
        public static readonly Color Gray = Color.gray;
        public static readonly Color Clear = Color.clear;

        // Custom Colors
        public static readonly Color LightRed = new Color(1.0f, 0.5f, 0.5f);
        public static readonly Color DarkRed = new Color(0.5f, 0.0f, 0.0f);
        public static readonly Color LightGreen = new Color(0.5f, 1.0f, 0.5f);
        public static readonly Color DarkGreen = new Color(0.0f, 0.5f, 0.0f);
        public static readonly Color LightBlue = new Color(0.5f, 0.5f, 1.0f);
        public static readonly Color DarkBlue = new Color(0.0f, 0.0f, 0.5f);
        public static readonly Color LightYellow = new Color(1.0f, 1.0f, 0.5f);
        public static readonly Color DarkYellow = new Color(0.5f, 0.5f, 0.0f);
        public static readonly Color LightCyan = new Color(0.5f, 1.0f, 1.0f);
        public static readonly Color DarkCyan = new Color(0.0f, 0.5f, 0.5f);
        public static readonly Color LightMagenta = new Color(1.0f, 0.5f, 1.0f);
        public static readonly Color DarkMagenta = new Color(0.5f, 0.0f, 0.5f);
        public static readonly Color LightOrange = new Color(1.0f, 0.7f, 0.4f);
        public static readonly Color DarkOrange = new Color(0.8f, 0.4f, 0.0f);
        public static readonly Color LightPurple = new Color(0.8f, 0.6f, 1.0f);
        public static readonly Color DarkPurple = new Color(0.5f, 0.0f, 0.5f);
        public static readonly Color LightGray = new Color(0.75f, 0.75f, 0.75f);
        public static readonly Color DarkGray = new Color(0.25f, 0.25f, 0.25f);
        public static readonly Color LightBrown = new Color(0.7f, 0.4f, 0.1f);
        public static readonly Color DarkBrown = new Color(0.4f, 0.2f, 0.05f);

        // Grays
        public static readonly Color Gainsboro = new Color(0.86f, 0.86f, 0.86f);
        public static readonly Color DarkSlateGray = new Color(0.18f, 0.31f, 0.31f);
        public static readonly Color LightSlateGray = new Color(0.47f, 0.53f, 0.6f);
        public static readonly Color SlateGray = new Color(0.44f, 0.5f, 0.56f);
        public static readonly Color DimGray = new Color(0.41f, 0.41f, 0.41f);
        public static readonly Color Silver = new Color(0.75f, 0.75f, 0.75f);

        // Additional Custom Colors
        public static readonly Color Pink = new Color(1.0f, 0.75f, 0.8f);
        public static readonly Color DarkPink = new Color(0.8f, 0.2f, 0.4f);
        public static readonly Color LightPink = new Color(1.0f, 0.85f, 0.9f);
        public static readonly Color LimeGreen = new Color(0.7f, 1.0f, 0.0f);
        public static readonly Color OliveGreen = new Color(0.5f, 0.5f, 0.0f);
        public static readonly Color Gold = new Color(1.0f, 0.84f, 0.0f);
        public static readonly Color Bronze = new Color(0.8f, 0.5f, 0.2f);
        public static readonly Color SkyBlue = new Color(0.53f, 0.81f, 0.92f);
        public static readonly Color DeepSkyBlue = new Color(0.0f, 0.75f, 1.0f);
        public static readonly Color Coral = new Color(1.0f, 0.5f, 0.31f);
        public static readonly Color Teal = new Color(0.0f, 0.5f, 0.5f);
        public static readonly Color Indigo = new Color(0.29f, 0.0f, 0.51f);
        public static readonly Color Violet = new Color(0.93f, 0.51f, 0.93f);
        public static readonly Color Mint = new Color(0.6f, 1.0f, 0.6f);
        public static readonly Color Peach = new Color(1.0f, 0.85f, 0.7f);
        public static readonly Color Lavender = new Color(0.9f, 0.9f, 0.98f);
        public static readonly Color MidnightBlue = new Color(0.1f, 0.1f, 0.44f);
        public static readonly Color ForestGreen = new Color(0.13f, 0.55f, 0.13f);
        public static readonly Color Tomato = new Color(1.0f, 0.39f, 0.28f);
        public static readonly Color Salmon = new Color(0.98f, 0.5f, 0.45f);
        public static readonly Color Khaki = new Color(0.94f, 0.9f, 0.55f);
        public static readonly Color Firebrick = new Color(0.7f, 0.13f, 0.13f);
        public static readonly Color GoldenRod = new Color(0.85f, 0.65f, 0.13f);
        public static readonly Color DodgerBlue = new Color(0.12f, 0.56f, 1.0f);
        public static readonly Color SeaGreen = new Color(0.18f, 0.55f, 0.34f);
        public static readonly Color HotPink = new Color(1.0f, 0.41f, 0.71f);
        public static readonly Color PaleGoldenRod = new Color(0.93f, 0.91f, 0.67f);
        public static readonly Color Plum = new Color(0.87f, 0.63f, 0.87f);
        public static readonly Color Thistle = new Color(0.85f, 0.75f, 0.85f);

        public static Color GetColor(string hashValue)
        {
            //int hashCode = hashValue.GetHashCode();

            //float r = ((hashCode >> 16) & 0xFF) / 255.0f;
            //float g = ((hashCode >> 8) & 0xFF) / 255.0f;
            //float b = (hashCode & 0xFF) / 255.0f;
            ColorUtility.TryParseHtmlString(hashValue, out Color color);
            return color;
        }
    }
}
