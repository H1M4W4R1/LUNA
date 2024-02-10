using System.Collections.Generic;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace H1M4W4R1.LUNA.Entities
{
    [BurstCompile]
    public class Hitbox : MonoBehaviour
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier = 1f;

        public NativeList<DamageVulnerability> vulnerabilities = new NativeList<DamageVulnerability>();
        public NativeList<DamageResistance> resistances = new NativeList<DamageResistance>();

        /// <summary>
        /// Executed when this hitbox gets hit with a weapon
        /// </summary> 
        public UnityEvent<DamageInfo> onHit;

        [BurstCompile]
        public void AddVulnerability(ref DamageVulnerability vulnerability) => vulnerabilities.Add(vulnerability);
        
        [BurstCompile]
        public void AddResistance(ref DamageResistance resistance) => resistances.Add(resistance);
        
        /// <summary>
        /// Deal damage to this hitbox
        /// </summary>
        public virtual void DealDamage(ref DamageInfo info)
        {
            onHit.Invoke(info);
        }
    }
}