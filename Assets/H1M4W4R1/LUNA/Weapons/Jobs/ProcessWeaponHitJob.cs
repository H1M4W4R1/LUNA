using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Jobs
{
    [BurstCompile]
    public struct ProcessWeaponHitJob : IJob, INativeDisposable
    {
        private NativeReference<WeaponData> _weaponData;
        private NativeReference<HitboxData> _hitboxData;
        private NativeReference<float3> _weaponPos;
        private NativeReference<quaternion> _weaponRot;
        private NativeReference<float3> _hitPos;
        private NativeReference<float3> _hitNormal;
        private NativeReference<DamageInfo> _damageInfo;


        public static void Prepare(in WeaponData weaponData, in HitboxData hitboxData, in float3 weaponPos,
            in quaternion weaponRot, in float3 hitPos, in float3 hitNormal, out ProcessWeaponHitJob job)
        {
            job = new ProcessWeaponHitJob()
            {
                _weaponData = new NativeReference<WeaponData>(weaponData, Allocator.TempJob),
                _hitboxData = new NativeReference<HitboxData>(hitboxData, Allocator.TempJob),
                _weaponPos = new NativeReference<float3>(weaponPos, Allocator.TempJob),
                _weaponRot = new NativeReference<quaternion>(weaponRot, Allocator.TempJob),
                _hitPos = new NativeReference<float3>(hitPos, Allocator.TempJob),
                _hitNormal = new NativeReference<float3>(hitNormal, Allocator.TempJob),
                _damageInfo = new NativeReference<DamageInfo>(default, Allocator.Domain)
            };
        }
        
        
        [BurstCompile]
        public void Execute()
        {
            Process(_weaponData.Value, _hitboxData.Value, _weaponPos.Value, _weaponRot.Value, _hitPos.Value, _hitNormal.Value,
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

        public void Dispose()
        {
            _weaponData.Dispose();
            _hitboxData.Dispose();
            _damageInfo.Dispose();
            _hitNormal.Dispose();
            _hitPos.Dispose();
            _weaponPos.Dispose();
            _weaponRot.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            return _weaponData.Dispose(_hitboxData.Dispose(
                _damageInfo.Dispose(
                    _hitNormal.Dispose(_hitPos.Dispose(_weaponPos.Dispose(_weaponRot.Dispose(inputDeps)))))));
        }

        public DamageInfo GetDamageInfo() =>
            _damageInfo.Value;
        
    }


}