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
        private NativeReference<WeaponMovementData> _movementData;

        [BurstCompatible]
        public static void Prepare(
            in WeaponMovementData movementData,
            out UpdateWeaponSpeedDataJob job)
        {
            job = new UpdateWeaponSpeedDataJob()
            {
                _movementData = new NativeReference<WeaponMovementData>(movementData, Allocator.TempJob)
            };
        }
        
        [BurstCompile]
        public void Execute()
        {
            var vectorList = _movementData.Value.weaponData.damageVectors;
            var mData = _movementData.Value;
            for (var index = 0; index < vectorList.Length; index++)
            {
                // Get vector and update data, then put updated data onto list
                var vector = vectorList[index]; // .get .cpy
                CalculateSpeedFactor(ref mData, ref vector);
                vectorList[index] = vector; // .ins
            }

            _movementData.Value = mData; // Update movement data
        }

        [BurstCompile]
        public void Dispose()
        {
            _movementData.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            _movementData.Dispose();
            return inputDeps;
        }
        
        [BurstCompile] [BurstCompatible]
        private void CalculateSpeedFactor(
            ref WeaponMovementData movementData, 
            ref WeaponDamageVector vector)
        {
            // Return if deltaTime is 0 to avoid division by zero in the speed calculation
            if (movementData.deltaTime == 0) return;

            // Compute position and speed
            var currentSpeed = (movementData.position - movementData.previousPosition) / movementData.deltaTime;

            var wData = movementData.weaponData;
            
            // Moving average formula using LERP
            var weight = wData.expectedAttackTime > 0 ? math.clamp(movementData.deltaTime / wData.expectedAttackTime, 0f, 1f) : 1f;
            vector.currentSpeed = math.lerp(vector.currentSpeed, currentSpeed, weight);
            vector.currentVelocity = math.length(vector.currentSpeed);
            vector.currentBaseDamage = CalculateBaseDamageForVector(movementData.weaponData, vector);
  
            // Update previous position and time for the next frame
            movementData.previousPosition = movementData.position;
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
                    return LinearDamageScale.Calculate(data.flatDamage, vector.currentVelocity);
                case DamageScaleMethod.Flat:
                    return FlatDamageScale.Calculate(data.flatDamage, vector.currentVelocity);
                case DamageScaleMethod.Quadratic:
                    return QuadraticDamageScale.Calculate(data.flatDamage, vector.currentVelocity);
                case DamageScaleMethod.Exponential:
                    return ExponentialDamageScale.Calculate(data.flatDamage, vector.currentVelocity);
            }

            Debug.LogError("[LUNA] Unknown damage type.");
            return 1f;
        }


        public float3 GetPreviousPosition() => _movementData.Value.previousPosition;
    }
}