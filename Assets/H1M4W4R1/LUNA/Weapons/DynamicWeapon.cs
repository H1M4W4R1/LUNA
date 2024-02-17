using System;
using H1M4W4R1.LUNA.Utilities.Jobs;
using H1M4W4R1.LUNA.Weapons.Jobs;
using H1M4W4R1.LUNA.Weapons.Jobs.Data;
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
        private float _lastTimeEvent = 0f;
        private IManagedJob _currentSpeedJob;
        
        [Tooltip("How long it should take wielder to swing this thing to deal base damage")]
        public float expectedAttackTime = 5.0f;

        protected new void Awake()
        {
            // Initialize this weapon
            base.Awake();
            
            // Initialize parameters
            _lastTimeEvent = Time.time;
            
            // Begin job
            if (_currentSpeedJob == null)
                UpdateJobDataEvent(true);
        }

        public void UpdateJobDataEvent(ManagedJob<object> job) => UpdateJobDataEvent(false);
        
        public void UpdateJobDataEvent(bool isFirst)
        {
            // Stash data if job has one
            if (!isFirst)
            {
                try
                {
                    _currentSpeedJob.Dispose();
                }
                catch (ObjectDisposedException)
                {
                    // Our game already died. We don't care at all.
                    // Probably manager killed our job due to crash or shutdown.
                    
                    // It might be a good idea to force-return to block execution of code below
                    return;
                }
            }
            
            // Process time pass
            var currentTime = Time.time;
            var dt = currentTime - _lastTimeEvent;
            _lastTimeEvent = currentTime;

            // Don't return to past
            if (dt < 0) return;

            var tt = transform;
            var weaponPosition = (float3) tt.position;
            var weaponRotation = (quaternion) tt.rotation;
            
            // Create new job
            UpdateWeaponSpeedDataJob.Prepare(
                new WeaponMovementData()
                {
                    weaponData = weaponData,
                    deltaTime = dt,
                    weaponPosition = weaponPosition,
                    weaponQuaternion = weaponRotation
                }, out var job);

            _currentSpeedJob =
                JobManager.ScheduleJob<object, UpdateWeaponSpeedDataJob, ManagedJob<object>>(job, 
                onCompleted: UpdateJobDataEvent);
        }

        protected void OnDestroy()
        {
            // Clean-up memory (force-finish the job)
            _currentSpeedJob?.Finish();
        }
       
    }
}