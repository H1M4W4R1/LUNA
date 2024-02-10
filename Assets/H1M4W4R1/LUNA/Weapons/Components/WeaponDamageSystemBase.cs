using System;
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
        public void Process(Hitbox hitbox, float3 position, float3 normalVector)
        {
            // Get collision information and damage type
            var dVector = _weapon.FindClosestDamageVector(position, normalVector);
            var damageType = dVector.damageType | _weapon.damageType;
            
            // Compute damage for this weapon
            var baseDamage = _weapon.GetSpeedDamageMultiplier(); // Speed multiplier and IDamageScaleMethod
            var damageMultVulnerability = 0f;
            
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
                        baseDamage *= vulnerability.damageMultiplier;
                        break;
                    case VulnerabilityScaling.Exponential:
                        baseDamage = math.pow(baseDamage, vulnerability.damageMultiplier);
                        break;
                }
            }

            // Mult damage by vulnerability
            if (damageMultVulnerability > 0f)
                baseDamage *= damageMultVulnerability;

            // And all other multipliers
            baseDamage *= hitbox.baseDamageMultiplier;

            // Deal damage
            var dmgInfo = new DamageInfo()
            {
                damageAmount = baseDamage,
                damageType = damageType,
                weapon = _weapon
            };

            hitbox.DealDamage(ref dmgInfo);
        }
    }
}