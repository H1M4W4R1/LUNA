using System;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Collections;
using UnityEngine;

namespace H1M4W4R1.LUNA.Entities
{
    [Serializable]
    public struct HitboxData
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier;

        public NativeList<DamageVulnerability> vulnerabilities;
        public NativeList<DamageResistance> resistances;

        public HitboxData(float baseDamageMultiplier)
        {
            this.baseDamageMultiplier = baseDamageMultiplier;
            vulnerabilities = new NativeList<DamageVulnerability>();
            resistances = new NativeList<DamageResistance>();
        }
    
    }
}