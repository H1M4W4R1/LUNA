using System.Collections.Generic;
using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Utilities.Jobs;
using H1M4W4R1.LUNA.Weapons.Jobs;
using H1M4W4R1.LUNA.Weapons.Jobs.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Components
{
    /// <summary>
    /// Represents basic weapon subsystem that is responsible for detecting damage occurence.
    /// It can be for example: when weapon collides with enemy or when enemy enters trigger.
    /// </summary>
    [BurstCompile]
    public abstract class WeaponDamageSystemBase : MonoBehaviour
    {
        private WeaponBase _weapon;
        protected Transform _transform;

        protected void Awake()
        {
            _weapon = GetComponent<WeaponBase>();
            _transform = transform;
        }

        protected void ProcessDamageEvent(Hitbox hitbox, float3 pos, quaternion rot, float3 hitPos, float3 hitNormal)
        {
            ProcessWeaponHitJob.Prepare(new WeaponHitData()
                {
                    weaponData = _weapon.GetData(),
                    hitboxData = hitbox.data,
                    hitNormal = hitNormal,
                    hitPos = hitPos,
                    weaponPos = pos,
                    weaponRotation = rot
                }, out var job);

            // Run the job
            JobManager.ScheduleJob<Hitbox, ProcessWeaponHitJob, ManagedJob<Hitbox>>(job, 
                hitbox, DamageHitbox);
        }

        private void DamageHitbox(ManagedJob<Hitbox> job)
        {
            var jobReference = job.GetJob<ProcessWeaponHitJob>();
            var dmgInfo = jobReference.GetDamageInfo();
            
            // Deal damage to hitbox
            job.GetReference().DealDamage(ref dmgInfo);
            
            // Get rid of trashy job
            job.Dispose();
        }

    }
}