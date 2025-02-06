using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using TankLike.Environment;
    using TankLike.Sound;

    [System.Serializable]
    public class TeleportationDestination
    {
        public RoomType RoomType;
        public string DestinationName;
    }

    public class TeleportationManager : MonoBehaviour, IManager
    {
        [SerializeField] private List<TeleportationDestination> _teleportationDestinations = new List<TeleportationDestination>();
        [SerializeField] private Audio _teleportAudio;

        public bool IsActive { get; private set; }
        public Dictionary<TeleportationDestination, Room> VisitedDestination { get; private set; } = new Dictionary<TeleportationDestination, Room>();

        private const float TELEPORTATION_EFFECT_DELAY = 0.5f;
        private const float REACTIVATE_PLAYERS_DELAY = 0.6f;

        #region IManager
        public void SetUp()
        {
            IsActive = true;
        }

        public void Dispose()
        {
            IsActive = false;

            VisitedDestination.Clear();
        }
        #endregion

        public bool IsSpecialRoom(RoomType type)
        {

            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return false;
            }

            var destination = _teleportationDestinations.Find(r => r.RoomType == type);
            
            return destination != null;
        }

        public void SetDestinationRoom(Room room)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            var destination = _teleportationDestinations.Find(r => r.RoomType == room.RoomType);

            if(destination != null)
            {
                if (!VisitedDestination.ContainsKey(destination))
                {
                    VisitedDestination.Add(destination, room);
                }
            } 
            else
            {
                if (IsSpecialRoom(room.RoomType))
                {
                    Debug.LogError($"{room.RoomType} is not a valid Teleportation Destination Room Type!");
                }
            }
        }

        public void Teleport(Room room)
        {
            if (!IsActive)
            {
                Debug.LogError("Manager " + GetType().Name + " is not active, and you're trying to use it!");
                return;
            }

            StartCoroutine(TeleportationRoutine(room));
        }

        private IEnumerator TeleportationRoutine(Room room)
        {
            float effectDuration = 0f;

            // Spawn effect
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                // Get player position
                var player = GameManager.Instance.PlayersManager.GetPlayer(i);
                Vector3 position = player.transform.position;

                player.Deactivate();

                // Spawn effect
                effectDuration = GameManager.Instance.VisualEffectsManager.Misc.PlayPlayerSpawnVFX(position);

                // play audio
                GameManager.Instance.AudioManager.Play(_teleportAudio);
            }

            yield return new WaitForSeconds(effectDuration);

            // Hide player visuals
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                // Get player position
                var player = GameManager.Instance.PlayersManager.GetPlayer(i);
                player.Visuals.HideVisuals();
            }

            // Teleport to the desired room
            GameManager.Instance.RoomsManager.TeleportToRoom(room);

            yield return new WaitForSeconds(TELEPORTATION_EFFECT_DELAY);


            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                // Get player position
                var player = GameManager.Instance.PlayersManager.GetPlayer(i);
                Vector3 position = player.transform.position;

                // Spawn effect
                var vfx = GameManager.Instance.VisualEffectsManager.Misc.PlayerSpawning;
                vfx.transform.SetPositionAndRotation(position, Quaternion.identity);
                vfx.gameObject.SetActive(true);
                vfx.Play();

                // play audio
                GameManager.Instance.AudioManager.Play(_teleportAudio);

                effectDuration = vfx.Particles.main.startLifetime.constant / 2;
            }

            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                // Get player position
                var player = GameManager.Instance.PlayersManager.GetPlayer(i);
                player.Visuals.ShowVisuals();
            }

            // Wait before reactivating the players
            yield return new WaitForSeconds(REACTIVATE_PLAYERS_DELAY);

            // Reactivate players
            for (int i = 0; i < PlayersManager.PlayersCount; i++)
            {
                GameManager.Instance.PlayersManager.GetPlayer(i).Activate();
            }

            // Reactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Activate();
            }

            // Enable inputs
            GameManager.Instance.InputManager.EnablePlayerInput();
        }
    }
}
