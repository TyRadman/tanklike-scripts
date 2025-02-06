using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankLike
{
    using UnitControllers;
    using Cam;
    using Environment;

    public class RoomsManager : MonoBehaviour, IManager
    {
        public bool IsActive { get; private set; }
        public List<Room> SpecialVisitedRooms { get; private set; } = new List<Room>();
        [field: SerializeField] public List<Room> Rooms { get; private set; }
        [field: SerializeField] public Room CurrentRoom { get; private set; }

        public System.Action<Room> OnRoomEntered;


        [SerializeField] private AbilityConstraint _onSwitchRoomsConstraints;

        [Header("References")]
        [SerializeField] private GameObject _roomsCoverPrefab;

        private GameObject _roomsCover;
        private ParticleSystem _weatherParticles;

        private const float SWITCH_ROOMS_DURATION = 0.5f;

        #region IManager
        public void SetUp()
        {
            IsActive = true;

            _roomsCover = Instantiate(_roomsCoverPrefab);
            _roomsCover.SetActive(false);

            _weatherParticles = Instantiate(GameManager.Instance.LevelGenerator.LevelData.WeatherVFX);
        }

        public void Dispose()
        {
            IsActive = false;

            _roomsCover = null;
            _weatherParticles = null;
        }
        #endregion

        public void AddRoom(Room room)
        {
            Rooms.Add(room);
        }

        public void SetCurrentRoom(Room room)
        {
            CurrentRoom = room;
            CurrentRoom.SetWasVisited(true);

            //if(_roomsCover != null)
            //{
            //    _roomsCover.transform.position = room.transform.position;
            //}

            if (_weatherParticles != null)
            {
                _weatherParticles.transform.position = room.transform.position;
            }

            GameManager.Instance.TeleportationManager.SetDestinationRoom(room);
        }

        public void SetupRooms()
        {
            Rooms.ForEach(r => r.SetUpRoom());
        }

        public void SwitchRoom(Room nextRoom, RoomGate enterGate = null)
        {
            StartCoroutine(SwitchRoomRoutine(nextRoom, enterGate));

            if (nextRoom.RoomType == RoomType.Boss)
            {

            }
            else
            {
                if (nextRoom is NormalRoom nextNormalRoom)
                {
                    nextNormalRoom.SetWaves();
                }
                else
                {
                    Debug.LogError("The room is not a normal room");
                }
            }
        }

        private IEnumerator SwitchRoomRoutine(Room nextRoom, RoomGate enterGate)
        {
            //List<UnitComponents> activePlayers = GameManager.Instance.PlayersManager.GetPlayers(true);
            List<UnitComponents> activePlayers = GameManager.Instance.PlayersManager.GetAllActivePlayerControllers();
            int activePlayersCount = activePlayers.Count;

            // disable the gates first to avoid having this be triggered by the second player before the room unloads
            CurrentRoom.DisableGates();

            GameManager.Instance.FadeUIController.StartFadeIn();
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeInDuration);

            // position the minimap at the top of the room
            GameManager.Instance.MinimapManager.PositionMinimapAtRoom(nextRoom.transform, nextRoom.RoomDimensions);

            // Deactivate players
            for (int i = 0; i < activePlayersCount; i++)
            {
                activePlayers[i].Deactivate();
                //GameManager.Instance.PlayersManager.GetPlayer(i).Constraints.ApplyConstraints(true, _onSwitchRoomsConstraints);
            }

            // Deactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Deactivate();
            }

            //GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.BulletsManager.DeactivateBullets();

            CurrentRoom.UnloadRoom();
            SetCurrentRoom(nextRoom);
            CurrentRoom.LoadRoom();

            // disable camera interpolation so that the players don't see our level's guts
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(false);

            GameManager.Instance.HUDController.OffScreenIndicator.Enable(false);

            // Respawn players
            for (int i = 0; i < activePlayersCount; i++)
            {
                Transform point = nextRoom.Spawner.SpawnPoints.GetRandomSpawnPoint();

                if (enterGate != null)
                {
                    point = enterGate.StartPoints[i];
                }

                activePlayers[i].PositionUnit(point);
                //Vector3 position = point.position;
                //position.y = 1f;
                //Quaternion rotation = Quaternion.LookRotation(point.forward);
                //activePlayers[i].transform.SetPositionAndRotation(position, Quaternion.identity);
                //((PlayerMovement)activePlayers[i].Movement).SetBodyRotation(rotation);
            }

            yield return new WaitForSeconds(SWITCH_ROOMS_DURATION);

            
            // Reactivate players
            for (int i = 0; i < activePlayersCount; i++)
            {
                activePlayers[i].Activate();
            }

            // Reactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Activate();
            }

            GameManager.Instance.FadeUIController.StartFadeOut();
            //GameManager.Instance.InputManager.EnablePlayerInput();

            OnRoomEntered?.Invoke(CurrentRoom);

            // change the camera constraints
            GameManager.Instance.CameraManager.SetCamerasLimits(nextRoom.CameraLimits);

            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeOutDuration);

            GameManager.Instance.CameraManager.EnableCamerasInterpolation(true);
            GameManager.Instance.HUDController.OffScreenIndicator.Enable(true);
        }

        public void TeleportToRoom(Room nextRoom, RoomGate enterGate = null)
        {
            StartCoroutine(TeleportToRoomRoutine(nextRoom, enterGate));
        }

        private IEnumerator TeleportToRoomRoutine(Room nextRoom, RoomGate enterGate)
        {
            List<UnitComponents> activePlayers = GameManager.Instance.PlayersManager.GetAllActivePlayerControllers();
            int activePlayersCount = activePlayers.Count;

            // disable the gates first to avoid having this be triggered by the second player before the room unloads
            CurrentRoom.DisableGates();

            GameManager.Instance.FadeUIController.StartFadeIn();
            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeInDuration);

            // position the minimap at the top of the room
            GameManager.Instance.MinimapManager.PositionMinimapAtRoom(nextRoom.transform, nextRoom.RoomDimensions);

            // Deactivate players
            for (int i = 0; i < activePlayersCount; i++)
            {
                activePlayers[i].Deactivate();
            }

            // Deactivate summons
            for (int i = 0; i < GameManager.Instance.SummonsManager.GetActiveSummonsCount(); i++)
            {
                GameManager.Instance.SummonsManager.GetSummon(i).Deactivate();
            }

            GameManager.Instance.InputManager.DisableInputs();
            GameManager.Instance.BulletsManager.DeactivateBullets();

            CurrentRoom.UnloadRoom();
            SetCurrentRoom(nextRoom);
            CurrentRoom.LoadRoom();

            // disable camera interpolation so that the players don't see our level's guts
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(false);
            GameManager.Instance.HUDController.OffScreenIndicator.Enable(false);

            // Respawn players
            for (int i = 0; i < activePlayersCount; i++)
            {
                Transform point = nextRoom.Spawner.SpawnPoints.GetRandomSpawnPoint();

                if (enterGate != null)
                {
                    point = enterGate.StartPoints[i];
                }

                activePlayers[i].PositionUnit(point);
            }

            yield return new WaitForSeconds(SWITCH_ROOMS_DURATION);
            GameManager.Instance.FadeUIController.StartFadeOut();
            OnRoomEntered?.Invoke(CurrentRoom);

            GameManager.Instance.CameraManager.SetCamerasLimits(nextRoom.CameraLimits);

            yield return new WaitForSeconds(GameManager.Instance.FadeUIController.FadeOutDuration);
            GameManager.Instance.CameraManager.EnableCamerasInterpolation(true);
            GameManager.Instance.HUDController.OffScreenIndicator.Enable(true);
        }

        public void OpenAllRooms()
        {
            Rooms.ForEach(r => r.OpenGates());
        }

        // Used for the testing scene only
        public void LoadBossRoom()
        {
            BossRoom bossRoom = (BossRoom)CurrentRoom;
            bossRoom.SetBossData(GameManager.Instance.LevelGenerator.RoomsBuilder.GetCurrentLevelData().BossData);
            bossRoom.SetBossSpawnPoint(CurrentRoom.transform);
            bossRoom.LoadRoom();
            bossRoom.OnRoomEnteredHandler();
        }
    }

    public enum RoomType
    {
        Normal = 0, Start = 1, Boss = 2, Shop = 3, SecretChallenge = 4, SecretShop = 5, BossGate = 6, Workshop = 7,
    }
}
