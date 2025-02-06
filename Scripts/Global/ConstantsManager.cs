using System.Collections;
using System.Collections.Generic;
using TankLike.Combat;
using TankLike.ScreenFreeze;
using TankLike.Sound;
using TankLike.UI.Inventory;
using TankLike.UnitControllers;
using UnityEngine;

namespace TankLike
{
    using UI.Notifications;

    /// <summary>
    /// Holds values that different class will need across the project
    /// </summary>
    [CreateAssetMenu(fileName = "Constants", menuName = MAIN_DIRECTORY + "Constants")]
    public class ConstantsManager : ScriptableObject
    {

        public const string MAIN_DIRECTORY = "TankLike/";
        public List<TankSide> Alignments;
        public LayerMask PlayersLayerMask;
        public LayerMask EnemiesLayerMask;
        public LayerMask PlayersAndEnemiesLayerMask;
        public string PlayerTag = "Player";
        public string EnemyTag = "Enemy";
        [Header("Tabs Variables")]
        public TabActivationSettings EnabledTabSettings;
        public TabActivationSettings DisabledTabSettings;
        [Header("Collectables")]
        public CollectablesInfo Collectables;
        [Header("Minimap")]
        public MinimapInfo Minimap;
        [Header("Enemies")]
        [Tooltip("The type/s of enemies that can't hold boss room keys.")]
        public List<EnemyType> NotKeyHolderEnemyTags;
        public ScreenFreezeData EnemyDeathFreezeData;
        public Audios Audios;

        [Header("Notifications")]
        public NotificationBarSettings_SO CoinsNotificationSettings;
    }

    [System.Serializable]
    public class CollectablesInfo
    {
        public AnimationCurve AttractionCurve;
        public List<AnimationCurve> BounceCurves;
        public float EnemyPreShrinkTime = 3f;
        public float BossPreShrinkTime = 3f;
        [field: SerializeField] public float DisplayDuration { get; private set; } = 5f;
        [field: SerializeField] public float BounceTime { get; private set; } = 0.8f;

        public float GetPreShrinkTime(bool boss)
        {
            return boss ? BossPreShrinkTime : EnemyPreShrinkTime;
        }
    }

    public enum TankAlignment
    {
        PLAYER, ENEMY, NEUTRAL
    }

    [System.Serializable]
    public class TankSide
    {
        public TankAlignment Alignment;
        public LayerMask LayerMask;
        public int LayerNumber;
        //public string Tag;
    }

    public enum GameTags
    {
        Player = 0, Enemy = 1, Bullet = 2
    }

    [System.Serializable]
    public class MinimapInfo
    {
        public AnimationClip SpawnAnimation;
        public AnimationClip KillAnimation;
        public AnimationClip PulseAnimation;
    }

    [System.Serializable]
    public class Audios
    {
        public Audio OnEmptyAmmoSound;
    }
}
