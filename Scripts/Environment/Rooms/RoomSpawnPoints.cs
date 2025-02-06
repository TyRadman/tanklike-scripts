using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TankLike.Environment
{
    using Utils;

    /// <summary>
    /// Manages spawn points within a room, allowing for adding, retrieving, and resetting spawn points.
    /// </summary>
    public class RoomSpawnPoints : MonoBehaviour
    {
        /// <summary>
        /// Represents a spawn point with a transform and a taken status.
        /// </summary>
        [System.Serializable]
        public class SpawnPoint
        {
            public Transform Point;
            [HideInInspector] public bool Taken = false;
        }

        [field: SerializeField] public List<SpawnPoint> Points { get; private set; } = new List<SpawnPoint>();

        /// <summary>
        /// Adds a new spawn point at the specified position and parent.
        /// </summary>
        /// <param name="position">The position of the new spawn point.</param>
        /// <param name="parent">The parent transform of the new spawn point.</param>
        public void AddSpawnPoint(Vector3 position, Transform parent)
        {
            Transform point = new GameObject("SpawnPoint").transform;
            point.parent = parent;
            point.position = position;
            SpawnPoint spawnPoint = new SpawnPoint() { Point = point, Taken = false };
            Points.Add(spawnPoint);

            //Debug.Log($"Room {GetComponent<Room>().name} has {Points.Count} points");
        }

        /// <summary>
        /// Returns a random spawn point from the list of available points.
        /// If all points are taken, it resets them and reuses them.
        /// </summary>
        /// <param name="markSpawnPointAsTaken">If true, marks the selected spawn point as taken.</param>
        /// <returns>The transform of the selected spawn point.</returns>
        public Transform GetRandomSpawnPoint(bool markSpawnPointAsTaken = true)
        {
            if (Points.Count <= 0)
            {
                Debug.LogError($"No points available in {gameObject.name}");
                return null;
            }

            SpawnPoint point;

            // if all the points are taken, then we reset them and reuse them
            if (!Points.Exists(p => !p.Taken))
            {
                SetAllPointsAsNotTaken();
                Debug.LogError($"Requests for points is higher than the number of points at room {gameObject.name}");
            }

            point = Points.FindAll(p => !p.Taken).RandomItem(true);

            if (markSpawnPointAsTaken)
            {
                point.Taken = true;
            }

            return point.Point;
        }

        /// <summary>
        /// Returns the furthest spawn point from the specified position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Transform GetFurthestSpawnPointFromPosition(Vector3 position, bool markSpawnPointAsTaken = true)
        {
            if (Points.Count <= 0)
            {
                Debug.LogError($"No points available in {gameObject.name}");
                return null;
            }

            SpawnPoint furthestPoint = Points
                .Where(p => !p.Taken)
                .OrderByDescending(p => Vector3.Distance(position, p.Point.position))
                .FirstOrDefault();

            if (furthestPoint == null)
            {
                Debug.LogError($"All points are taken in {gameObject.name}");
                return null;
            }

            if (markSpawnPointAsTaken)
            {
                furthestPoint.Taken = true;
            }

            return furthestPoint.Point;
        }

        public List<Transform> GetFurthestSpawnPointsFromPosition(Vector3 position, int count, bool markSpawnPointAsTaken = true)
        {
            Points.ForEach(p => p.Taken = false);

            if (Points.Count <= 0)
            {
                Debug.LogError($"No points available in {gameObject.name}");
                return null;
            }

            List<Transform> furthestPoints = Points
                .Where(p => !p.Taken)
                .OrderByDescending(p => Vector3.Distance(position, p.Point.position))
                .Take(count)
                .Select(p =>
                {
                    if (markSpawnPointAsTaken)
                    {
                        p.Taken = true;
                    }

                    return p.Point;
                }).ToList();

            if (furthestPoints.Count == 0)
            {
                Debug.LogError($"All points are taken in {gameObject.name}");
                return null;
            }

            return furthestPoints;
        }

        /// <summary>
        /// Resets all spawn points to be available (not taken).
        /// </summary>
        public void SetAllPointsAsNotTaken()
        {
            Points.ForEach(p => p.Taken = false);
        }

        /// <summary>
        /// Returns the closest spawn point to the specified position that is at least the specified minimum distance away.
        /// If the specified order is not available, it returns the closest available point.
        /// </summary>
        /// <param name="position">The position to find the closest spawn point to.</param>
        /// <param name="order">The order of the closest point to return (0-based).</param>
        /// <param name="minDistance">The minimum distance the spawn point must be from the specified position.</param>
        /// <returns>The transform of the closest spawn point.</returns>
        public Transform GetClosestSpawnPointToPosition(Vector3 position, int order, float minDistance = 0f)
        {
            List<SpawnPoint> closestPoints = Points
                .Where(p => Vector3.Distance(position, p.Point.position) > minDistance)
                .OrderBy(p => Vector3.Distance(position, p.Point.position))
                .ToList();

            if (closestPoints.Count == 0)
            {
                return null;
            }

            // Start from the requested order and move backwards if necessary
            for (int i = order; i >= 0; i--)
            {
                if (i < closestPoints.Count)
                {
                    return closestPoints[i].Point;
                }
            }

            // If no valid point is found, return the closest point
            return closestPoints[0].Point;
        }

        #region Optimized approach
        // The following methods are an optimized approach using QuickSelect to find the k-th closest point.
        // They are commented out for now but can be used if performance becomes an issue.

        //public Transform GetClosestSpawnPointToPosition(Vector3 position, int order, float minDistance = 0f)
        //{
        //    List<SpawnPoint> closestPoints = new List<SpawnPoint>();

        //    foreach (var point in Points)
        //    {
        //        float distance = Vector3.Distance(position, point.Point.position);

        //        if (distance > minDistance)
        //        {
        //            // Add to the list of closest points
        //            closestPoints.Add(point);
        //        }
        //    }

        //    // Use a partial selection to get the 'order'-th closest point
        //    SpawnPoint closestPoint = QuickSelect(closestPoints, position, order);

        //    return closestPoint?.Point;
        //}

        //private SpawnPoint QuickSelect(List<SpawnPoint> points, Vector3 position, int order)
        //{
        //    if (points == null || points.Count == 0)
        //    {
        //        return null;
        //    }

        //    order = Mathf.Min(order, points.Count - 1);
        //    int right = points.Count - 1;
        //    int left = 0;

        //    while (left <= right)
        //    {
        //        int pivotIndex = Partition(points, left, right, position);

        //        if (pivotIndex == order)
        //        {
        //            return points[pivotIndex];
        //        }
        //        else if (pivotIndex < order)
        //        {
        //            left = pivotIndex + 1;
        //        }
        //        else
        //        {
        //            right = pivotIndex - 1;
        //        }
        //    }

        //    return null;
        //}

        //private int Partition(List<SpawnPoint> points, int left, int right, Vector3 position)
        //{
        //    float pivotDistance = Vector3.Distance(position, points[right].Point.position);
        //    int partitionIndex = left;

        //    for (int i = left; i < right; i++)
        //    {
        //        if (Vector3.Distance(position, points[i].Point.position) <= pivotDistance)
        //        {
        //            // Swap points[i] with points[partitionIndex]
        //            (points[i], points[partitionIndex]) = (points[partitionIndex], points[i]);
        //            partitionIndex++;
        //        }
        //    }

        //    // Swap pivot with points[partitionIndex]
        //    (points[right], points[partitionIndex]) = (points[partitionIndex], points[right]);

        //    return partitionIndex;
        //}
        #endregion
    }
}

