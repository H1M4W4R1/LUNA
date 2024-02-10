using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons
{
    /// <summary>
    /// Represents dynamic in-world weapon eg. sword or axe.
    /// Beware that for multi-part weapons like nun-cha-ku all parts should have DynamicWeapon assigned.
    /// </summary>
    [BurstCompile]
    public class DynamicWeapon : WeaponBase
    {
        private float3 _previousPosition; // Cache of last weapon position in 3D WORLD space
        private float3 _averagedSpeed; // Average speed of weapon in las expectedAttackTime [s]
        
        [Tooltip("How long it should take wielder to swing this thing to deal base damage")]
        public float expectedAttackTime = 5.0f;
        
        [BurstCompile]
        public override float3 GetRecentSpeed() => _averagedSpeed;

        [BurstCompile]
        public override float GetSpeedDamageMultiplier() => _damageScaleMethod.CalculateScaleFrom(math.length(_averagedSpeed));

        protected new void Awake()
        {
            // Initialize this weapon
            base.Awake();
            
            // Initialize parameters
            _previousPosition = transform.position;
            _averagedSpeed = float3.zero;
        }

        protected void Update()
        {
            var dt = Time.deltaTime;
            CalculateSpeedFactor(dt);
        }

        [BurstCompile]
        private void CalculateSpeedFactor(float deltaTime)
        {
            // Compute position and speed
            var currentPosition = (float3) transform.position;
            var currentSpeed = (currentPosition - _previousPosition) / deltaTime;

            // Update averaged speed
            _averagedSpeed = CalculateMovingAverage(currentSpeed, deltaTime);

            // Update previous position and time for the next frame
            _previousPosition = currentPosition;
        }
        
        [BurstCompile]
        private float3 CalculateMovingAverage(float3 currentSpeed, float deltaTime)
        {
            // Moving average formula using LERP 
            var weight = math.clamp(deltaTime / expectedAttackTime, 0f, 1f);
            return math.lerp(_averagedSpeed, currentSpeed, weight);
        }
    }
}