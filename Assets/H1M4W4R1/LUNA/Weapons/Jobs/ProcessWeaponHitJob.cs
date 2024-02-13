using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Damage;
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
    /// Represents a job to be called when weapon hits something
    /// Should be called from OnTrigger / OnCollision or respectable system.
    /// Should be called once (when event starts to happen).
    /// </summary>
    [BurstCompile]
    public struct ProcessWeaponHitJob : IJob, INativeDisposable
    {
        private NativeReference<WeaponHitData> _hitData;
        private NativeReference<DamageInfo> _damageInfo; 

        public static void Prepare(in WeaponHitData hitData, out ProcessWeaponHitJob job)
        {
            job = new ProcessWeaponHitJob()
            {
                _hitData = new NativeReference<WeaponHitData>(hitData, Allocator.TempJob),
                _damageInfo = new NativeReference<DamageInfo>(default, Allocator.TempJob)
            };
        }

        [BurstCompile]
        public void Execute()
        {
            var data = _hitData.Value;
            
            Process(data.weaponData, data.hitboxData, data.weaponPos, data.weaponRotation, data.hitPos, data.hitNormal,
                out var dInfo);
            _damageInfo.Value = dInfo;
        }

        /// <summary>
        /// Process this damage system 
        /// </summary>
        [BurstCompile]
        public void Process(
            in WeaponData weaponData,
            in HitboxData hitbox,
            in float3 objectPosition,
            in quaternion objectRotation,
            in float3 eventPosition,
            in float3 normalVector,
            out DamageInfo dmgInfo)
        {
            // Get vector information
            WeaponDamageVectorCalculation.FindClosestDamageVector(weaponData, objectPosition, objectRotation,
                eventPosition, normalVector, out var dVector);

            // Evaluate damage type via combination
            var damageType = dVector.damageType | weaponData.damageType;

            // Compute damage for this weapon
            var damage = weaponData.GetSpeedDamageMultiplier(); // Speed multiplier and IDamageScaleMethod
            var damageMultVulnerability = 0f;
            var damageMultResistance = 0f;

            // INFO: NVM forgot that Burst does not compile from managed objects.

            // Check for vulnerabilities
            foreach (var vulnerability in hitbox.vulnerabilities)
            {
                if (!vulnerability.IsVulnerableTo(damageType)) continue;

                // Compute scaling
                switch (weaponData.vulnerabilityScaling)
                {
                    case VulnerabilityScaling.None:
                        if (vulnerability.damageMultiplier > damageMultVulnerability)
                            damageMultVulnerability = vulnerability.damageMultiplier;
                        break;
                    case VulnerabilityScaling.Additive:
                        damageMultVulnerability += vulnerability.damageMultiplier;
                        break;
                    case VulnerabilityScaling.Multiplicative:
                        damageMultVulnerability = damageMultVulnerability == 0f
                            ? vulnerability.damageMultiplier
                            : damageMultVulnerability * vulnerability.damageMultiplier;
                        // damage *= vulnerability.damageMultiplier;
                        break;
                    case VulnerabilityScaling.Exponential:
                        damageMultVulnerability = damageMultVulnerability == 0f
                            ? vulnerability.damageMultiplier
                            : math.pow(damageMultVulnerability, vulnerability.damageMultiplier);
                        // damage = math.pow(damage, vulnerability.damageMultiplier);
                        break;
                }
            }

            // Check for resistances
            foreach (var resistance in hitbox.resistances)
            {
                if (!resistance.IsResistantTo(damageType)) continue;

                // Compute scaling
                switch (weaponData.vulnerabilityScaling)
                {
                    case VulnerabilityScaling.None:
                        if (resistance.damageAntiMultiplier > damageMultResistance)
                            damageMultResistance = resistance.damageAntiMultiplier;
                        break;
                    case VulnerabilityScaling.Additive:
                        damageMultResistance += resistance.damageAntiMultiplier;
                        break;
                    case VulnerabilityScaling.Multiplicative:
                        damageMultResistance = damageMultResistance == 0f
                            ? resistance.damageAntiMultiplier
                            : damageMultResistance * resistance.damageAntiMultiplier;
                        // damage /= resistance.damageAntiMultiplier;
                        break;
                    case VulnerabilityScaling.Exponential:
                        damageMultResistance = damageMultResistance == 0f
                            ? resistance.damageAntiMultiplier
                            : math.pow(damageMultResistance, resistance.damageAntiMultiplier);
                        // damage = math.pow(damage, 1f / resistance.damageAntiMultiplier);
                        break;
                }
            }

            // Mult damage by vulnerability
            if (damageMultVulnerability > 0f)
                damage *= damageMultVulnerability;
            if (damageMultResistance > 0f)
                damage /= damageMultResistance;

            // And all other multipliers
            damage *= hitbox.baseDamageMultiplier;

            // Deal damage
            dmgInfo = new DamageInfo()
            {
                damageAmount = damage,
                damageType = damageType,
            };
        }

        [BurstCompile]
        public void Dispose()
        {
            _hitData.Dispose();
            _damageInfo.Dispose();
  
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            _hitData.Dispose();
            _damageInfo.Dispose();
            return inputDeps;
        }

        public DamageInfo GetDamageInfo() =>
            _damageInfo.Value;
        
    }


}