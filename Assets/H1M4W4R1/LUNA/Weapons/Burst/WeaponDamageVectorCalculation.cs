using System;
using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Burst
{
    [BurstCompile] [BurstCompatible]
    public static class WeaponDamageVectorCalculation
    {
        /// <summary>
        /// Finds the closest damage vector for a given weapon.
        /// </summary>
        /// <param name="data">The weapon data.</param>
        /// <param name="position">The position of the weapon.</param>
        /// <param name="rotation">The rotation of the weapon.</param>
        /// <param name="collisionPoint">The point of collision.</param>
        /// <param name="collisionNormal">The normal of the collision surface.</param>
        /// <param name="closestStruct">The closest damage vector to the collision point.</param>
        /// <exception cref="ArgumentException">Thrown when the weapon does not have any damage vectors.</exception>
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
            if(data.damageVectors.Length < 1)
                throw new ArgumentException("[LUNA] Weapon must have at least one damage vector for calculation.");

            closestStruct = default;
            var invertedCollisionNormal = -collisionNormal;
            var minScore = float.MaxValue;

            foreach (var currentStruct in data.damageVectors)
            {
                var angleDifference = math.dot(currentStruct.GetVectorForRotation(rotation), invertedCollisionNormal);
                angleDifference = data.directionMatters ? angleDifference : math.abs(angleDifference);
                var distance = math.distance(currentStruct.startPoint + position, collisionPoint);
                var score = data.angleWeight * (1 - angleDifference) + data.distanceWeight * distance;

                if (score > minScore) continue;
                minScore = score;
                closestStruct = currentStruct;
            }
        }
    }
}