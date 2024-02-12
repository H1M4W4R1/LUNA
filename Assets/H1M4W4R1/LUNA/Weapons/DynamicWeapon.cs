using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons
{
    /// <summary>
    /// Represents dynamic in-world weapon eg. sword or axe.
    /// Beware that for multi-part weapons like nun-cha-ku all parts should have DynamicWeapon assigned.
    /// </summary>
    public class DynamicWeapon : WeaponBase
    {
        private float3 _previousPosition; // Cache of last weapon position in 3D WORLD space
        
        [Tooltip("How long it should take wielder to swing this thing to deal base damage")]
        public float expectedAttackTime = 5.0f;
        
        public override float3 GetRecentSpeed() => _weaponData.currentSpeed;

        public override float GetSpeedDamageMultiplier() => _damageScaleMethod.CalculateScaleFrom(math.length(_weaponData.currentSpeed));

        protected new void Awake()
        {
            // Initialize this weapon
            base.Awake();
            
            // Initialize parameters
            _previousPosition = transform.position;
            _weaponData.currentSpeed = float3.zero;
        }

        protected new void Update()
        {
            base.Update();
            
            var pos = (float3) transform.position;
            var dt = Time.deltaTime;

            UpdateWeaponSpeedDataJob.Prepare(dt, ref _weaponData, _weaponData.currentSpeed, pos,
                ref _previousPosition, out var job);
            
            // Run job and wait for completion
            job.Schedule().Complete();
            job.Dispose();
        }

    
        
        
    }
}