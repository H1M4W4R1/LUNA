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
        /// <summary>
        /// Executed when this hitbox gets hit with a weapon
        /// </summary> 
        public UnityEvent<DamageInfo> onHit;

        public HitboxData data = new HitboxData(1f);
        
        [BurstCompile]
        public void AddVulnerability(ref DamageVulnerability vulnerability) => data.vulnerabilities.Add(vulnerability);
        
        [BurstCompile]
        public void AddResistance(ref DamageResistance resistance) => data.resistances.Add(resistance);
        
        /// <summary>
        /// Deal damage to this hitbox
        /// </summary>
        public virtual void DealDamage(ref DamageInfo info)
        {
            onHit.Invoke(info);
        }
    }
}