using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Burst
{
    [BurstCompile] [BurstCompatible]
    public static class WeaponDamageVectorCalculation
    {
        /// <summary>
        /// Find closest damage vector based on attack point and normalized direction
        /// </summary>
        [BurstCompile]
        [BurstCompatible]
        public static void FindClosestDamageVector(
            in WeaponData data,
            in float3 position,
            in quaternion rotation,
            in float3 collisionPoint,
            in float3 collisionNormal,
            out WeaponDamageVector closestStruct)
        {
            closestStruct = default(WeaponDamageVector);

            if(data.damageVectors.Length < 1)
                Debug.LogError("[LUNA] Weapon must have at least one damage vector. Otherwise it's useless!");

            // Invert collision normal for calculation (use anti-normal)
            var cAntiNormal = -collisionNormal;
            var minScore = float.MaxValue;

            // INFO: damageVectors must be NativeList (not Collections.Generic.List)
            foreach (var currentStruct in data.damageVectors)
            {
                // Calculate angle difference (cosine similarity between **normalized** vectors)
                var angleDifference = 
                    math.dot(currentStruct.GetVectorForRotation(rotation), cAntiNormal);

                // Ignore direction if does not matter
                if (!data.directionMatters)
                    angleDifference = math.abs(angleDifference);

                // Calculate distance (startPoint is in local space, collision is in world space)
                var distance = math.distance(currentStruct.startPoint + position, collisionPoint);

                // Combine angle and distance using specified weights
                var score = data.angleWeight * (1 - angleDifference) + data.distanceWeight * distance;

                // Update closest struct if the current score is smaller
                if (score < minScore)
                {
                    minScore = score;
                    closestStruct = currentStruct;
                }
            }
        }
    }
}