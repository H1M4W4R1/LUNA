using System;
using System.Collections.Generic;
using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
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
        private List<LinkedJob<Hitbox>> _jobs = new List<LinkedJob<Hitbox>>();

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

            // Register job
            _jobs.Add(new LinkedJob<Hitbox>(job, job.Schedule(), hitbox));
        }

        private void OnDestroy()
        {
            // Dispose all running jobs
            foreach (var jobReference in _jobs)
            {
                jobReference.handle.Complete();
                if (jobReference.job is INativeDisposable disposable)
                    disposable.Dispose();
            }
        }

        protected void Update()
        {
            // Create a temporary list to store jobs to be removed
            var jobsToRemove = new List<LinkedJob<Hitbox>>();

            foreach (var jobReference in _jobs)
            {
                if (!jobReference.handle.IsCompleted) continue;

                // Stop compiler from complaining
                jobReference.handle.Complete();

                // Get job and damage information, then get rid of obsolete data to prevent issues
                var job = (ProcessWeaponHitJob) jobReference.job;
                var dmgInfo = job.GetDamageInfo();

                // Deal damage
                jobReference.refValue.DealDamage(ref dmgInfo);
                job.Dispose();

                // Add job to the removal list instead of removing it directly
                jobsToRemove.Add(jobReference);
            }

            // Remove all completed jobs from the original list
            foreach (var job in jobsToRemove)
                _jobs.Remove(job);

        }
    }
}