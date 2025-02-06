using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TankLike.Environment
{
    using Cam;
    using Sound;
    using Attributes;
    using Environment.MapMaker;

    /// <summary>
    /// Represents a room in the game, managing its setup, loading, unloading, and interactions such as gates and spawn points.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [field: SerializeField, InSelf] public RoomGatesInfo GatesInfo { get; protected set; }
        [field: SerializeField, InSelf] public RoomUnitSpawner Spawner { get; protected set; }
        [field: SerializeField, InSelf] public RoomSpawnablesController Spawnables { get; private set; }
        [field: SerializeField] public RoomType RoomType { get; protected set; }
        [field: SerializeField] public CameraLimits CameraLimits { get; protected set; }
        [field: SerializeField] public MapTiles_SO MapTiles { get; set; }

        public Vector2Int Location { get; set; }
        public Vector2Int RoomDimensions { get; private set; }
        public bool WasVisited { get; private set; }


        [SerializeField, InSelf] protected NavMeshSurface _surface;
        [Header("Settings")]
        [SerializeField] protected Audio _openGateAudios;
        [SerializeField] protected Audio _closeGateAudios;

        /// <summary>
        /// Sets up the room. This method can be overridden by derived classes to provide specific setup logic.
        /// </summary>
        public virtual void SetUpRoom()
        {
            Spawner.SetUp(this);
        }

        /// <summary>
        /// Loads the room, making it active and enabling its gates.
        /// </summary>
        public virtual void LoadRoom()
        {
            // display the room
            gameObject.SetActive(true);
            GatesInfo.Gates.ForEach(g => g.Gate.EnableGate());
        }

        public void SetRoomDimesions(Vector2Int dimensions)
        {
            RoomDimensions = dimensions;
        }

        /// <summary>
        /// Disables all gates in the room.
        /// </summary>
        public void DisableGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.DisableGate());
        }

        /// <summary>
        /// Unloads the room, making it inactive.
        /// </summary>
        public virtual void UnloadRoom()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Handles logic when the room is entered. This method can be overridden by derived classes to provide specific logic.
        /// </summary>
        public virtual void OnRoomEnteredHandler()
        {
        }

        /// <summary>
        /// Opens all gates in the room.
        /// </summary>
        public void OpenGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.OpenGate());
        }

        /// <summary>
        /// Closes all gates in the room.
        /// </summary>
        public void CloseGates()
        {
            GatesInfo.Gates.ForEach(g => g.Gate.CloseGate());
        }

        /// <summary>
        /// Plays the audio associated with opening gates.
        /// </summary>
        public void PlayOpenGateAudio()
        {
            GameManager.Instance.AudioManager.Play(_openGateAudios);
        }

        /// <summary>
        /// Bakes the room's NavMesh surface.
        /// </summary>
        public void BakeRoom()
        {
            _surface.BuildNavMesh();
        }

        /// <summary>
        /// Sets the type of the room.
        /// </summary>
        /// <param name="type">The type of the room.</param>
        public void SetRoomType(RoomType type)
        {
            RoomType = type;
        }

        /// <summary>
        /// Sets the camera limits for the room.
        /// </summary>
        /// <param name="limits">The camera limits to set.</param>
        public void SetCameraLimits(CameraLimits limits)
        {
            // set the edge points of the room as the limits
            CameraLimits = new CameraLimits();
            CameraLimits.HorizontalLimits = limits.HorizontalLimits;
            CameraLimits.VerticalLimits = limits.VerticalLimits;
        }

        

        /// <summary>
        /// Gets the list of barrels in the room.
        /// </summary>
        /// <returns>A list of barrels in the room.</returns>
        public List<Transform> GetBarrels()
        {
            return null;
        }

        /// <summary>
        /// Gets a random spawn point in the room.
        /// </summary>
        /// <returns>The position of a random spawn point.</returns>
        public Vector3 GetRandomSpawnPoint()
        {
            return Spawner.SpawnPoints.GetRandomSpawnPoint(false).position;
        }

        /// <summary>
        /// Gets the closest spawn point to the specified position in the room.
        /// </summary>
        /// <param name="position">The position to find the closest spawn point to.</param>
        /// <param name="order">The order of the closest point to return (0-based).</param>
        /// <returns>The position of the closest spawn point.</returns>
        public Vector3 GetClosestSpawnPointToPosition(Vector3 position, int order)
        {
            return Spawner.SpawnPoints.GetClosestSpawnPointToPosition(position, order).position;
        }

        /// <summary>
        /// Set the WasVisited flag to room when the room is first visited
        /// </summary>
        public void SetWasVisited(bool value)
        {
            WasVisited = value;
        }
    }
}