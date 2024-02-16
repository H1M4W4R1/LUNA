using System.Collections.Generic;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Damage;
using UnityEngine;
using UnityEngine.Events;

namespace H1M4W4R1.LUNA.Entities
{
    public class Hitbox : MonoBehaviour
    {
        /// <summary>
        /// Executed when this hitbox gets hit with a weapon
        /// </summary> 
        public UnityEvent<DamageInfo> onHit;

        public HitboxData data = new HitboxData();
        
        // Serializable variables (managed)
        public List<DamageVulnerability> vulnerabilities = new List<DamageVulnerability>();
        public List<DamageResistance> resistances = new List<DamageResistance>();

        private void Awake()
        {
            // Register data
            data.RegisterVulnerabilities(vulnerabilities);
            data.RegisterResistances(resistances);
        }

        /// <summary>
        /// Deal damage to this hitbox
        /// </summary>
        public virtual void DealDamage(ref DamageInfo info)
        {
            onHit?.Invoke(info);

#if UNITY_EDITOR
            Debug.Log($"RECEIVED: {info.damageType} with amount of {info.damageAmount}");
#endif
        }
    }
}