using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Jobs.Data;
using H1M4W4R1.LUNA.Weapons.Scaling;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Jobs
{
    /// <summary>
    /// Represents a request to update weapon speed data.
    /// Should be called "Every frame" (either game or system frame)
    /// </summary>
    [BurstCompatible] [BurstCompile] 
    public struct UpdateWeaponSpeedDataJob : IJob, INativeDisposable
    {
        private WeaponMovementData _movementData;

        [BurstCompatible]
        public static void Prepare(
            in WeaponMovementData movementData,
            out UpdateWeaponSpeedDataJob job)
        {
            job = new UpdateWeaponSpeedDataJob()
            {
                _movementData = movementData
            };
        }
        
        [BurstCompile]
        public void Execute()
        {
            var vectorList = _movementData.weaponData.damageVectors;

            for (var index = 0; index < vectorList.Length; index++)
            {
                // Get vector and update data, then put updated data onto list
                var vector = vectorList[index]; // .get .cpy
                CalculateSpeedFactor(_movementData, ref vector);
                vectorList[index] = vector; // .ins
            }
        }

        [BurstCompile]
        public void Dispose()
        {
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return inputDeps;
        }
        
        [BurstCompile] [BurstCompatible]
        private void CalculateSpeedFactor(
            in WeaponMovementData movementData, 
            ref WeaponDamageVector vector)
        {
            // Return if deltaTime is 0 to avoid division by zero in the speed calculation
            if (movementData.deltaTime == 0) return;

            // Rotate vector point with weapon, then use it to calc current position
            vector.currentPosition = movementData.weaponPosition + math.rotate(movementData.weaponQuaternion, vector.startPoint);
            
            // Compute position delta and speed
            var currentSpeed = (vector.currentPosition - vector.previousPosition) / movementData.deltaTime;

            var wData = movementData.weaponData;
            
            // Moving average formula using LERP
            var weight = wData.expectedAttackTime > 0 ? math.clamp(movementData.deltaTime / wData.expectedAttackTime, 0f, 1f) : 1f;
            vector.currentVelocity = math.lerp(vector.currentVelocity, currentSpeed, weight);
            vector.currentSpeed = math.length(vector.currentVelocity);
            vector.currentBaseDamage = CalculateBaseDamageForVector(movementData.weaponData, vector);

            vector.previousPosition = vector.currentPosition;
        }

        /// <summary>
        /// Calculates the base damage for a given weapon and damage vector.
        /// </summary>
        /// <param name="data">The weapon data.</param>
        /// <param name="vector">The weapon damage vector</param>
        /// <returns>The calculated base damage.</returns>
        /// <remarks>
        /// The calculation method depends on the damage scale method specified in the weapon data:
        /// - Linear: Uses the LinearDamageScale.Calculate method.
        /// - Flat: Uses the FlatDamageScale.Calculate method.
        /// - Quadratic: Uses the QuadraticDamageScale.Calculate method.
        /// - Exponential: Uses the ExponentialDamageScale.Calculate method.
        /// If the damage scale method is unknown, an error is logged and a default damage of 1 is returned.
        /// </remarks>
        [BurstCompile]
        private float CalculateBaseDamageForVector(in WeaponData data, in WeaponDamageVector vector)
        {
            switch (data.damageScaleMethod)
            {
                case DamageScaleMethod.Linear:
                    return LinearDamageScale.Calculate(data.flatDamage, vector.currentSpeed);
                case DamageScaleMethod.Flat:
                    return FlatDamageScale.Calculate(data.flatDamage, vector.currentSpeed);
                case DamageScaleMethod.Quadratic:
                    return QuadraticDamageScale.Calculate(data.flatDamage, vector.currentSpeed);
                case DamageScaleMethod.Exponential:
                    return ExponentialDamageScale.Calculate(data.flatDamage, vector.currentSpeed);
            }

            Debug.LogError("[LUNA] Unknown damage type.");
            return 1f;
        }
    }
}