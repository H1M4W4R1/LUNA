using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Jobs
{
    [BurstCompatible] [BurstCompile] 
    public struct UpdateWeaponSpeedDataJob : IJob, INativeDisposable
    {
        private float _deltaTime;
        private NativeReference<WeaponData> _weaponData;
        private NativeReference<float3> _position;
        private NativeReference<float3> _previousPosition;

        [BurstCompatible]
        public static void Prepare(
            in float dt, 
            in WeaponData weaponData,
            in float3 position,
            in float3 previousPosition,
            out UpdateWeaponSpeedDataJob job)
        {
            job = new UpdateWeaponSpeedDataJob()
            {
                _deltaTime = dt,
                _weaponData = new NativeReference<WeaponData>(weaponData, Allocator.TempJob),
                _position = new NativeReference<float3>(position, Allocator.TempJob),
                _previousPosition = new NativeReference<float3>(previousPosition, Allocator.TempJob)
            };
        }
        
        public void Execute()
        {
            // Update weapon data
            CalculateSpeedFactor(_position.Value, _deltaTime);
        }

        public void Dispose()
        {
            _weaponData.Dispose();
            _position.Dispose();
            _previousPosition.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps) =>
            _weaponData.Dispose(_previousPosition.Dispose(_position.Dispose(inputDeps)));
        
        [BurstCompile] [BurstCompatible]
        private void CalculateSpeedFactor(in float3 currentPosition, in float deltaTime)
        {
            // Compute position and speed
            var currentSpeed = (currentPosition - _previousPosition.Value) / deltaTime;

            // Update averaged speed
            var value = _weaponData.Value;
            
            // Moving average formula using LERP
            var weight = value.expectedAttackTime > 0 ? math.clamp(deltaTime / value.expectedAttackTime, 0f, 1f) : 1f;
            value.currentSpeed = math.lerp(value.currentSpeed, currentSpeed, weight);
            
            // Update weapon value
            _weaponData.Value = value;
            
            // Update previous position and time for the next frame
            _previousPosition.Value = currentPosition;
        }

        public float3 GetCurrentSpeed() =>
            _weaponData.Value.currentSpeed;

        public float3 GetPreviousPosition() => _previousPosition.Value;

    }
}