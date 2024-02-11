using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Components
{
    /// <summary>
    /// Represents basic weapon subsystem that is responsible for detecting damage occurence.
    /// It can be for example: when weapon collides with enemy or when enemy enters trigger.
    /// </summary>
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
        /// </summary>
        [BurstCompile]
        public void Process(in HitboxData hitbox, in float3 position, in float3 normalVector, out DamageInfo dmgInfo)
        {
            // Get collision information and damage type
            var dVector = _weapon.FindClosestDamageVector(position, normalVector);
            var damageType = dVector.damageType | _weapon.damageType;
            
            // Compute damage for this weapon
            var damage = _weapon.GetSpeedDamageMultiplier(); // Speed multiplier and IDamageScaleMethod
            var damageMultVulnerability = 0f;
            var damageMultResistance = 0f;
            
            // INFO: This seems to be compiled with burst, but List<> should not be compatible with it,
            // maybe Burst does it differently?
            
            // Check for vulnerabilities
            foreach (var vulnerability in hitbox.vulnerabilities)
            {
                if (!vulnerability.IsVulnerableTo(damageType)) continue;

                // Compute scaling
                switch (_weapon.vulnerabilityScaling)
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
                switch (_weapon.vulnerabilityScaling)
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