using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Jobs
{
    [BurstCompatible] [BurstCompile] 
    public struct UpdateWeaponSpeedDataJob : IJob, INativeDisposable
    {
        private float _deltaTime;
        private NativeReference<WeaponData> _weaponData;
        private NativeReference<float3> _currentSpeed;
        private NativeReference<float3> _position;
        private NativeReference<float3> _previousPosition;

        [NotBurstCompatible]
        public static UpdateWeaponSpeedDataJob Prepare(in float dt, ref WeaponData weaponData,
            in float3 currentSpeed, in float3 position, ref float3 previousPosition)
        {
            Prepare(dt, ref weaponData, currentSpeed, position, ref previousPosition, out var job);
            return job;
        }
        
        [BurstCompatible]
        public static void Prepare(
            in float dt, 
            ref WeaponData weaponData,
            in float3 currentSpeed, 
            in float3 position,
            ref float3 previousPosition,
            out UpdateWeaponSpeedDataJob job)
        {
            job = new UpdateWeaponSpeedDataJob()
            {
                _deltaTime = dt,
                _weaponData = new NativeReference<WeaponData>(weaponData, Allocator.Domain),
                _currentSpeed = new NativeReference<float3>(currentSpeed, Allocator.Domain),
                _position = new NativeReference<float3>(position, Allocator.Domain),
                _previousPosition = new NativeReference<float3>(previousPosition, Allocator.Domain)
            };
        }
        
        public void Execute()
        {
            // Update weapon data
            var posValue = _previousPosition.Value;
            CalculateSpeedFactor(_position.Value, _deltaTime, ref posValue);
            _previousPosition.Value = posValue;
        }

        public void Dispose()
        {
            _weaponData.Dispose();
            _currentSpeed.Dispose();
            _position.Dispose();
            _previousPosition.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps) =>
            _weaponData.Dispose(_currentSpeed.Dispose(_previousPosition.Dispose(_position.Dispose(inputDeps))));
        
        [BurstCompile] [BurstCompatible]
        private void CalculateSpeedFactor(in float3 currentPosition, in float deltaTime, ref float3 prevPositionData)
        {
            // Compute position and speed
            var currentSpeed = (currentPosition - prevPositionData) / deltaTime;

            // Update averaged speed
            var value = _weaponData.Value;
            WeaponCalculation.UpdateWeaponSpeed(ref value, currentSpeed, deltaTime);
            _weaponData.Value = value;

            // Update previous position and time for the next frame
            prevPositionData = currentPosition;
        }
    }
}