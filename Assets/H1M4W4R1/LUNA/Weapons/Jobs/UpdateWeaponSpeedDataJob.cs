using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Jobs.Data;
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
            // Update weapon data
            var value = _movementData.Value;
            CalculateSpeedFactor(ref value);
            _movementData.Value = value;
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
        private void CalculateSpeedFactor(ref WeaponMovementData movementData)
        {
            // Compute position and speed
            var currentSpeed = (movementData.position - movementData.previousPosition) / movementData.deltaTime;

            var wData = movementData.weaponData;
            
            // Moving average formula using LERP
            var weight = wData.expectedAttackTime > 0 ? math.clamp(movementData.deltaTime / wData.expectedAttackTime, 0f, 1f) : 1f;
            wData.currentSpeed = math.lerp(wData.currentSpeed, currentSpeed, weight);

            movementData.weaponData = wData;
            
            // Update previous position and time for the next frame
            movementData.previousPosition = movementData.position;
        }

        public float3 GetCurrentSpeed() =>
            _movementData.Value.weaponData.currentSpeed;

        public float3 GetPreviousPosition() => _movementData.Value.previousPosition;

    }
}