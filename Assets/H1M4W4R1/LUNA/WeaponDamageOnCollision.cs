using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA
{
    /// <summary>
    /// Deals weapon damage when weapon collides with damageable object
    /// </summary>
    [RequireComponent(typeof(WeaponBase))]
    public class WeaponDamageOnCollision : MonoBehaviour
    {
        private WeaponBase _weapon;
        
        private void Awake()
        {
            _weapon = GetComponent<WeaponBase>();
        }

        private void OnCollisionEnter(Collision other)
        {
            // Get HitBox component
            var hitbox = other.gameObject.GetComponent<Hitbox>();

            // Only first contact matters
            var cPoint = other.GetContact(0);
            
            // Get collision information and damage type
            var dVector = _weapon.FindClosestDamageVector(cPoint.point, cPoint.normal);
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