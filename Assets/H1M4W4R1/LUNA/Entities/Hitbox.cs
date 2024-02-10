using System.Collections.Generic;
using H1M4W4R1.LUNA.Weapons.Damage;
using UnityEngine;
using UnityEngine.Events;

namespace H1M4W4R1.LUNA.Entities
{
    public class Hitbox : MonoBehaviour
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier = 1f;

        public readonly List<DamageVulnerability> vulnerabilities = new List<DamageVulnerability>();

        /// <summary>
        /// Executed when this hitbox gets hit with a weapon
        /// </summary> 
        public UnityEvent<DamageInfo> onHit;

        /// <summary>
        /// Add damage vulnerability
        /// </summary>
        public void AddVulnerability(ref DamageVulnerability v) => vulnerabilities.Add(v);
        
        /// <summary>
        /// Deal damage to this hitbox
        /// </summary>
        public virtual void DealDamage(ref DamageInfo info)
        {
            onHit.Invoke(info);
        }
    }
}