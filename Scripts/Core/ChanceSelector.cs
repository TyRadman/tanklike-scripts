using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TankLike
{
    public static class ChanceSelector
    {
        /// <summary>
        /// Selects an item from the list based on the chances provided
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chances"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static T SelectByChance<T>(Dictionary<Func<T>, float> chances, Func<T> defaultValue = null)
        {
            float totalProbability = chances.Values.Sum();

            var normalizedChances = chances.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value / totalProbability
                );

            float chance = UnityEngine.Random.value;
            float cumulativeProbability = 0f;

            foreach (var kvp in normalizedChances)
            {
                cumulativeProbability += kvp.Value;

                if (chance < cumulativeProbability)
                {
                    T selected = kvp.Key();

                    if (selected != null)
                    {
                        return selected;
                    }
                }
            }

            throw new InvalidOperationException("No item selected");
        }
    }
}
