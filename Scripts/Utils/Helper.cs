using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TankLike.Utils
{
    /// <summary>
    /// A utility class providing various helper methods for common tasks such as random value generation, list manipulation, and vector operations.
    /// </summary>
    public static class Helper
    {
#if UNITY_EDITOR
        public static T[] GetAssetsOfType<T>() where T : ScriptableObject
        {
            string className = typeof(T).Name;
            return AssetDatabase.FindAssets($"t:{className}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToArray();
        }
#endif

        public static bool IsRuntimeInstance(this Object obj)
        {
            return obj != null && obj.GetInstanceID() < 0;
        }

        public static string Size(this string text, int size)
        {
            return $"<size={size}>{text}</size>";
        }

        public static float GetDistanceTo(this Transform transform, Vector3 position)
        {
            return Vector3.Distance(transform.position, position);
        }

        public static bool IsEmpty(this IList list)
        {
            return list == null || list.Count == 0;
        }

        /// <summary>
        /// Returns a random value between the negative and positive values of the given value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetRandomValue(this float value)
        {
            return Random.Range(-value, value);
        }

        public static Vector2 Vector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static bool HasFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : System.Enum
        {
            // we need to convert it to int to be able to use bitwise operations since enums are basically integers while the TEnum is what we define it to be.
            int intValue = System.Convert.ToInt32(value);
            int intFlag = System.Convert.ToInt32(flag);

            return (intValue & intFlag) != 0;
        }

        public static T RandomItem<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Returns a random float value between the vector's x and y values.
        /// </summary>
        /// <param name="_value">The Vector2 containing the range (x, y).</param>
        /// <returns>A random float value between x and y.</returns>
        public static float RandomValue(this Vector2 _value)
        {
            return Random.Range(_value.x, _value.y);
        }

        /// <summary>
        /// Returns a random integer value between the vector's x and y values.
        /// </summary>
        /// <param name="_value"></param>
        /// <param name="_float"></param>
        /// <returns></returns>
        public static bool HasFloatInRange(this Vector2 _value, float _float)
        {
            return _float >= _value.x && _float <= _value.y;
        }

        /// <summary>
        /// Returns a random integer value between the vector's x and y values. The check is inclusive.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsInRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Linearly interpolates between the x and y values of the vector based on the given t value.
        /// </summary>
        /// <param name="_value">The Vector2 containing the range (x, y).</param>
        /// <param name="_tValue">The interpolation factor (0 to 1).</param>
        /// <returns>The interpolated float value.</returns>
        public static float Lerp(this Vector2 _value, float _tValue)
        {
            return Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        /// <summary>
        /// Linearly interpolates between the x and y values of the vector and returns the result as an integer.
        /// </summary>
        /// <param name="_value">The Vector2Int containing the range (x, y).</param>
        /// <param name="_tValue">The interpolation factor (0 to 1).</param>
        /// <returns>The interpolated integer value.</returns>
        public static int Lerp(this Vector2Int _value, float _tValue)
        {
            return (int)Mathf.Lerp(_value.x, _value.y, _tValue);
        }

        /// <summary>
        /// Returns a random integer value between the vector's x and y values.
        /// </summary>
        /// <param name="_value">The Vector2Int containing the range (x, y).</param>
        /// <returns>A random integer value between x and y (inclusive).</returns>
        public static int RandomValue(this Vector2Int _value)
        {
            return Random.Range(_value.x, _value.y + 1);
        }

        /// <summary>
        /// Returns a random Vector2 with each component between the negative and positive values of the given vector.
        /// </summary>
        /// <param name="_value">The Vector2 containing the range for each component.</param>
        /// <returns>A random Vector2 within the specified range.</returns>
        public static Vector2 RandomVector2(this Vector2 _value)
        {
            return new Vector2(Random.Range(-_value.x, _value.x), Random.Range(-_value.y, _value.y));
        }

        #region List Methods
        /// <summary>
        /// Shuffles the list randomly.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int k = Random.Range(0, i);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }

        /// <summary>
        /// Returns a random item from the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select a random item from.</param>
        /// <returns>A random item from the list.</returns>
        public static T RandomItem<T>(this IList<T> list)
        {
            if(list.Count <= 0)
            {
                return default(T); // Returns null for reference types, default value for value types.
            }

            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a random item from the list and optionally removes it.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to select a random item from.</param>
        /// <param name="removeItem">If true, removes the selected item from the list.</param>
        /// <returns>A random item from the list.</returns>
        public static T RandomItem<T>(this IList<T> list, bool removeItem)
        {
            T selectedItem = list[Random.Range(0, list.Count)];

            if (removeItem)
            {
                list.Remove(selectedItem);
            }

            return selectedItem;
        }

        /// <summary>
        /// Logs an error message indicating a wrong skill holder.
        /// </summary>
        /// <param name="gameObjectName">The name of the game object.</param>
        /// <param name="expectedType">The expected type of the skill.</param>
        /// <param name="passType">The actual type of the skill passed.</param>
        public static void LogWrongSkillHolder(string gameObjectName, string expectedType, string passType)
        {
            Debug.LogError($"Wrong skill passed to {gameObjectName}. Expected {expectedType} and got {passType} instead.");
        }

        /// <summary>
        /// Converts an array to a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to convert.</param>
        /// <returns>A list containing the elements of the array.</returns>
        public static List<T> ArrayToList<T>(T[] array)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                list.Add(array[i]);
            }

            return list;
        }

        /// <summary>
        /// Creates a duplicate of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to duplicate.</param>
        /// <returns>A new list containing the same elements as the original list.</returns>
        public static List<T> Duplicate<T>(this List<T> list)
        {
            List<T> newList = new List<T>();
            list.ForEach(i => newList.Add(i));
            return newList;
        }
        #endregion

        /// <summary>
        /// Colors the string value based on the color selected.
        /// </summary>
        /// <param name="text">The text to color.</param>
        /// <param name="_color">The new color of the string.</param>
        /// <returns>The colored string.</returns>
        public static string Color(this string text, Color _color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(_color)}>{text}</color>";
        }

        /// <summary>
        /// Logs an error indicating that the expected input action is null.
        /// </summary>
        public static void ThrowInputActionError()
        {
            Debug.LogError("Expected input action is null.");
        }

        /// <summary>
        /// Returns the tag of the opposite side of the given tag.
        /// </summary>
        /// <param name="tag">The tag of the requester.</param>
        /// <returns>The opposing tag.</returns>
        public static string GetOpposingTag(string tag)
        {
            return tag == TanksTag.Player.ToString() ? TanksTag.Enemy.ToString() : TanksTag.Player.ToString();
        }

        /// <summary>
        /// Returns the opposite direction of the given direction.
        /// </summary>
        /// <param name="direction">The direction to get the opposite of.</param>
        /// <returns>The opposite direction.</returns>
        public static Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.None;
            }
        }

        /// <summary>
        /// Returns the opposing alignment of the given tank alignment.
        /// </summary>
        /// <param name="alignment">The alignment to get the opposite of.</param>
        /// <returns>The opposing alignment.</returns>
        public static TankAlignment GetOpposingTag(TankAlignment alignment)
        {
            return alignment == TankAlignment.PLAYER ? TankAlignment.ENEMY : TankAlignment.PLAYER;
        }

        /// <summary>
        /// Adds a value to the given value and ensures it stays within the specified range.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="valueToAdd">The value to add.</param>
        /// <param name="minRange">The minimum range.</param>
        /// <param name="maxRange">The maximum range.</param>
        /// <returns>The resulting value within the specified range.</returns>
        public static int AddInRange(int value, int valueToAdd, int minRange, int maxRange)
        {
            value += valueToAdd;

            if (value < minRange)
            {
                value = maxRange;
            }
            else if (value > maxRange)
            {
                value = minRange;
            }
            else
            {
                value = Mathf.Clamp(value, minRange, maxRange);
            }

            return value;
        }

        /// <summary>
        /// Adds a value to the given value and ensures it stays within the specified range.
        /// </summary>
        /// <param name="value">The initial value.</param>
        /// <param name="valueToAdd">The value to add.</param>
        /// <param name="maxRange">The maximum range.</param>
        /// <returns>The resulting value within the specified range.</returns>
        public static int AddInRange(int value, int valueToAdd, int maxRange)
        {
            int minRange = 0;
            value += valueToAdd;

            if (value < minRange)
            {
                value = maxRange;
            }
            else if (value > maxRange)
            {
                value = minRange;
            }
            else
            {
                value = Mathf.Clamp(value, minRange, maxRange);
            }

            return value;
        }

        /// <summary>
        /// Gets the world position of the mouse cursor.
        /// </summary>
        /// <param name="mouseColliderLayerMask">The layer mask to use for the raycast.</param>
        /// <returns>The world position of the mouse cursor.</returns>
        public static Vector3 GetMouseWorldPosition(LayerMask mouseColliderLayerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 999f, mouseColliderLayerMask))
            {
                return hit.point;
            }
            else
            {
                return Vector3.zero;
            }
        }

        /// <summary>
        /// Returns a random point inside a sphere with the given center and radius.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>A random point inside the sphere.</returns>
        public static Vector3 GetRandomPointInsideSphere(Vector3 center, float radius)
        {
            return center + Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// Returns a new vector with the given values.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="x">The new x value (optional).</param>
        /// <param name="y">The new y value (optional).</param>
        /// <param name="z">The new z value (optional).</param>
        /// <returns>A new vector with the specified values.</returns>
        public static Vector3 Where(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            return new Vector3(
                float.IsNaN(x) ? vector.x : x,
                float.IsNaN(y) ? vector.y : y,
                float.IsNaN(z) ? vector.z : z);
        }

        /// <summary>
        /// Adds given values to the vector.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="x">The value to add to the x component (optional).</param>
        /// <param name="y">The value to add to the y component (optional).</param>
        /// <param name="z">The value to add to the z component (optional).</param>
        /// <returns>A new vector with the added values.</returns>
        public static Vector3 Add(this Vector3 vector, float x = float.NaN, float y = float.NaN, float z = float.NaN)
        {
            return new Vector3(
                vector.x + (float.IsNaN(x) ? 0 : x),
                vector.y + (float.IsNaN(y) ? 0 : y),
                vector.z + (float.IsNaN(z) ? 0 : z));
        }

        /// <summary>
        /// Checks if a point is inside a sphere with the given center and radius.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>True if the point is inside the sphere, false otherwise.</returns>
        public static bool IsPointInsideSphere(Vector3 point, Vector3 center, float radius)
        {
            float distance = Vector3.Distance(point, center);
            return distance <= radius;
        }

        /// <summary>
        /// Returns the input icon as a sprite tag.
        /// </summary>
        /// <param name="index">The index of the input icon.</param>
        /// <returns>The input icon as a sprite tag.</returns>
        public static string GetInputIcon(int index)
        {
            return $"<sprite={index}>";
        }

        /// <summary>
        /// Represents a simple spline with three control points.
        /// </summary>
        public struct SimpleSpline
        {
            private Vector3 _a0, _a1, _a2;

            public SimpleSpline(Vector3 a0, Vector3 a1, Vector3 a2)
            {
                _a0 = a0;
                _a1 = a1;
                _a2 = a2;
            }

            /// <summary>
            /// Evaluates the spline at the given t value.
            /// </summary>
            /// <param name="t">The t value (0 to 1).</param>
            /// <returns>The evaluated point on the spline.</returns>
            public Vector3 Evaluate(float t)
            {
                return Mathf.Pow(t, 2f) * (_a2 - 2f * _a1 + _a0) + t * (2f * _a1 - 2f * _a0) + _a0;
            }

            /// <summary>
            /// Evaluates the tangent of the spline at the given t value.
            /// </summary>
            /// <param name="t">The t value (0 to 1).</param>
            /// <returns>The evaluated tangent on the spline.</returns>
            public Vector3 EvaluateTangent(float t)
            {
                return 2f * t * (_a2 - 2f * _a1 + _a0) + (2f * _a1 - 2f * _a0);
            }
        }

        /// <summary>
        /// Checks if a manager is active and throws an exception if it is not.
        /// </summary>
        /// <param name="isActive">Indicates if the manager is active.</param>
        /// <param name="type">The type of the manager.</param>
        public static void CheckForManagerActivity(bool isActive, System.Type type)
        {
            if (!isActive)
            {
                Debug.LogError($"Manager {type.Name} is not active, and you're trying to use it!");
                throw new System.Exception();
            }
        }

        /// <summary>
        /// Checks if a component is valid and throws an exception if it is not.
        /// </summary>
        /// <param name="isActive">Indicates if the component is valid.</param>
        /// <param name="type">The type of the component.</param>
        public static void CheckForComponentValidity(bool isActive, System.Type type)
        {
            if (!isActive)
            {
                Debug.LogError($"You passed a wrong Components type {type.Name}");
                throw new System.Exception();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Returns the first asset found in the project files of type T.
        /// </summary>
        /// <typeparam name="T">The type of the asset to be found.</typeparam>
        /// <returns>The first asset found of type T.</returns>
        public static T FindAssetFromProjectFiles<T>() where T : ScriptableObject
        {
            T asset = null;
            string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).FullName}");

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return asset;
        }

        /// <summary>
        /// Returns all assets found in the project files of type T.
        /// </summary>
        /// <typeparam name="T">The type of the assets to be found.</typeparam>
        /// <param name="pathToSearch">The path to search for the assets (optional).</param>
        /// <returns>A list of all assets found of type T.</returns>
        public static List<T> FindAllAssetFromProjectFiles<T>(string pathToSearch = "") where T : Object
        {
            List<T> assets = new List<T>();
            string[] guids;

            if (pathToSearch == "")
            {
                guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            }
            else
            {
                guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { pathToSearch });
            }

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets.Add(asset);
            }

            return assets;
        }
#endif

        public static void StopCoroutineSafe(this MonoBehaviour monoBehavior, Coroutine coroutine)
        {
            if (coroutine == null)
            {
                return;
            }

            monoBehavior.StopCoroutine(coroutine);
        }

        public static void PlayAnimation(this MonoBehaviour monoBehavior, Animation animation, AnimationClip clip)
        {
            if (animation == null || clip == null)
            {
                return;
            }

            if (animation.isPlaying)
            {
                animation.Stop();
            }

            animation.clip = clip;
            animation.Play();
        }

        public static void PlayAnimation(this MonoBehaviour monoBehavior, Animation animation)
        {
            if (animation == null)
            {
                return;
            }

            if (animation.isPlaying)
            {
                animation.Stop();
            }

            animation.Play();
        }

        public static void LogSetUpNullReferences(System.Type type)
        {
            Debug.LogError($"{type.Name} is null and you're trying to setting it up");
        }

        public static void LogWrongAbilityProperty(System.Type type)
        {
            Debug.LogError($"Expected ability of type {type.Name}.");
        }

        /// <summary>
        /// Prints "You passed a wrong Components type" with the name of the type.
        /// </summary>
        /// <param name="type"></param>
        public static void LogWrongComponentsType(System.Type type)
        {
            Debug.LogError("You passed a wrong Components type " + type.Name);
        }

        public const float ROTATION_CORRECTION_THRESHOLD = 0.02f;
        public const float ROTATION_MINIMUM_THRESHOLD = 0.8f;

        public static float GetRotationAmount(Vector3 originPosition, Vector3 targetPosition, Vector3 originForward)
        {
            Vector3 direction = (targetPosition - originPosition).normalized;
            Vector3 tankForward = originForward.normalized;
            Vector3 crossDot = Vector3.Cross(tankForward, direction);
            float rotationAmount;
            float cross = crossDot.y;

            Vector2 dotProductAccuracy = new Vector2(ROTATION_MINIMUM_THRESHOLD, ROTATION_CORRECTION_THRESHOLD);
            Vector2 frameRateRange = new Vector2Int(30, 60);
            int fp = (int)(1f / Time.deltaTime);

            float t = Mathf.InverseLerp(frameRateRange.x, frameRateRange.y, fp);
            float dotProductThreshold = 1 - dotProductAccuracy.Lerp(t);

            if (cross > dotProductThreshold)
            {
                rotationAmount = 1f;
            }
            else if (cross < -dotProductThreshold)
            {
                rotationAmount = -1f;
            }
            else
            {
                if (cross > 0)
                {
                    rotationAmount = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(dotProductThreshold, 0f, cross));
                }
                else if (cross < 0)
                {
                    rotationAmount = Mathf.Lerp(-1f, 0f, Mathf.InverseLerp(-dotProductThreshold, 0f, cross));
                }
                else
                {
                    rotationAmount = 0f;
                }
            }

            return rotationAmount;
        }

        /// <summary>
        /// Checks if a random chance succeeds and, if successful, returns a random value within the specified range.
        /// </summary>
        /// <param name="chance">The probability (0.0 to 1.0) that the action will succeed.</param>
        /// <param name="range">The range (min and max) from which to pick the random value if the chance succeeds.</param>
        /// <param name="result">The output value if the chance succeeds; otherwise, remains unchanged.</param>
        /// <returns>True if the chance succeeded and the result was updated; otherwise, false.</returns>
        public static bool CheckRandomChance(float chance)
        {
            return Random.Range(0f, 1f) < chance;
        }

        /// <summary>
        /// Checks if a random chance succeeds and, if successful, returns a random value within the specified range.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChanceSuccessful(this float value)
        {
            return Random.Range(0f, 1f) < value;
        }
    }
}