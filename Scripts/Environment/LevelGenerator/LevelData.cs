using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment.LevelGeneration
{
    using Environment.MapMaker;
    using Sound;
    using UnitControllers;

    /// <summary>
    /// Holds the values that distinct a level from another.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData_NAME", menuName = Directories.LEVEL + "Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public MapTileStyler Styler { get; private set; }
        [field: SerializeField] public MapTiles_SO BossRoom { get; private set; }
        [field: SerializeField] public BossRoomGate BossGate { get; private set; }
        [field: SerializeField] public BossData BossData { get; private set; }
        [field: SerializeField] public Audio LevelMusic { get; private set; }
        [field: SerializeField] public List<MapTiles_SO> MapPools { get; private set; }
        [field: SerializeField] public Vector2Int DroppersRange { get; private set; }
        [HideInInspector] public float CratesToRocksChance = 0.3f;
        [field: SerializeField] public Vector2Int ExplosivesRangePerRoom { get; private set; }
        [field: SerializeField] public Vector2Int GrassRangePerRoom { get; private set; } = new Vector2Int(5, 20);
        [field: SerializeField] public Vector2Int DestructibleWallsRangePerRoom { get; private set; } = new Vector2Int(5, 20);
        [field: SerializeField] public List<WaveData> Waves { get; private set; }
        [field: SerializeField, Header("Effects")] public ParticleSystem WeatherVFX { get; private set; }
    }
}
