﻿using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Jobs;
using H1M4W4R1.LUNA.Weapons.Jobs.Data;
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
        
        public override float3 GetRecentSpeed() => weaponData.currentSpeed;

        public override float GetSpeedDamageMultiplier() => _damageScaleMethod.CalculateScaleFrom(math.length(weaponData.currentSpeed));

        protected new void Awake()
        {
            // Initialize this weapon
            base.Awake();
            
            // Initialize parameters
            _previousPosition = transform.position;
            weaponData.currentSpeed = float3.zero;
        }

        protected new void Update()
        {
            base.Update();
            
            var pos = (float3) transform.position;
            var dt = Time.deltaTime;

            // Run the job and update data
            UpdateWeaponSpeedDataJob.Prepare(
                new WeaponMovementData()
                {
                    weaponData = weaponData,
                    deltaTime = dt,
                    position = pos,
                    previousPosition = _previousPosition
                }, out var job);
            
            // Run job and wait for completion
            job.Schedule().Complete();
            weaponData.currentSpeed = job.GetCurrentSpeed();
            _previousPosition = job.GetPreviousPosition();
            
            job.Dispose();
        }

    
        
        
    }
}