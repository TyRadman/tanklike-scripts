using System.Collections.Generic;
using UnityEngine;

namespace TankLike.Environment
{
    public class NormalRoom : Room
    {
        [field: SerializeField, Header("Settings")] public int SpawningCapacity { get; private set; }
        public bool HasEnemies { get; private set; } = true;
        public bool SpawnedEnemies { get; set; } = false;

        public override void SetUpRoom()
        {
            base.SetUpRoom();

            foreach (GateInfo gate in GatesInfo.Gates)
            {
                gate.Gate.Setup(HasEnemies, this);
            }

            Spawner.SetParticleWait(GameManager.Instance.VisualEffectsManager.Misc.EnemySpawning.Particles.main.startLifetime.constant);
        }

        public override void LoadRoom()
        {
            base.LoadRoom();
        }

        public override void UnloadRoom()
        {
            base.UnloadRoom();
        }

        public override void OnRoomEnteredHandler()
        {
            base.OnRoomEnteredHandler();

            if (!Spawner.HasEnemies())
            {
                return;
            }

            // Close every gate in the room
            foreach (var gate in GatesInfo.Gates)
            {
                gate.Gate.CloseGate();
            }

            Spawner.CheckAndAddKeyHolder();

            Spawner.ActivateSpawnedEnemies();

            GameManager.Instance.AudioManager.Play(_closeGateAudios);
            GameManager.Instance.CameraManager.Zoom.SetToFightZoom();
            GameManager.Instance.EnemiesManager.SetFightActivated(true);

        }

        public void SetHasEnemies(bool hasEnemies)
        {
            HasEnemies = hasEnemies;
        }

        internal void SetWaves()
        {
            if (HasEnemies && !SpawnedEnemies)
            {
                List<EnemyWave> waves = GameManager.Instance.EnemiesManager.GetWaves(this);
                Spawner.SetRoomEnemyWaves(waves);
                SpawnedEnemies = true;
            }
        }

        public void FlagAsKeyHolder()
        {
            Spawner.HasKey = true;
        }
    }
}
