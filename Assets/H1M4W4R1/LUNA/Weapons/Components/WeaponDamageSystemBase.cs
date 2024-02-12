using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
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

        /// <summary>
        /// Process this damage system
        /// TODO: implement Burst compatibility
        /// </summary>
        [BurstCompile]
        [NotBurstCompatible]
        public unsafe void Process(
            in WeaponData weaponData, 
            in HitboxData hitbox, 
            in float3 objectPosition,
            in quaternion objectRotation,
            in float3 eventPosition, 
            in float3 normalVector, 
            out DamageInfo dmgInfo)
        {
            // Get vector information
            WeaponDamageVectorCalculation.FindClosestDamageVector(&weaponData, objectPosition, objectRotation,
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
                        damageMultVulnerability = damageMultVulnerability == 0f ? 
                            vulnerability.damageMultiplier : math.pow(damageMultVulnerability, vulnerability.damageMultiplier);
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
                        damageMultResistance = damageMultResistance == 0f ? 
                            resistance.damageAntiMultiplier : math.pow(damageMultResistance, resistance.damageAntiMultiplier);
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
                weapon = _weapon
            };
        }
    }
}