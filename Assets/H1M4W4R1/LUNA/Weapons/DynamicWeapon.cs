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
        private JobHandle _updateSpeedJobHandle;
        private UpdateWeaponSpeedDataJob _updateSpeedJob;
        private bool _hadJob = false;
        
        private float3 _previousPosition; // Cache of last weapon position in 3D WORLD space
        
        [Tooltip("How long it should take wielder to swing this thing to deal base damage")]
        public float expectedAttackTime = 5.0f;

        protected new void Awake()
        {
            // Initialize this weapon
            base.Awake();
            
            // Initialize parameters
            _previousPosition = transform.position;
        }

        protected void OnDestroy()
        {
            // Clean-up memory
            if (_hadJob)
            {
                _updateSpeedJobHandle.Complete();
                _updateSpeedJob.Dispose();
            }
        }
        
        protected void Update()
        {
            var pos = (float3) transform.position;
            var dt = Time.deltaTime;
            
            // Run job section
            if (_updateSpeedJobHandle.IsCompleted || !_hadJob)
            {
                // Copy data
                if (_hadJob)
                {
                    // Make sure job is completed
                    _updateSpeedJobHandle.Complete();
                    
                    _previousPosition = _updateSpeedJob.GetPreviousPosition();
                    
                    // Delete old job after data gathering
                    _updateSpeedJob.Dispose();
                }
                
                // Create new job
                UpdateWeaponSpeedDataJob.Prepare(
                    new WeaponMovementData()
                    {
                        weaponData = weaponData,
                        deltaTime = dt,
                        position = pos,
                        previousPosition = _previousPosition
                    }, out _updateSpeedJob);
                
                _updateSpeedJobHandle = _updateSpeedJob.Schedule(_updateSpeedJobHandle);
                _hadJob = true;
            }
        }
    }
}