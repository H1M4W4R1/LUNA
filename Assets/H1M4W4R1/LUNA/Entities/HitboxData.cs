using System;
using System.Collections.Generic;
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

        public List<DamageVulnerability> vulnerabilities;
        public List<DamageResistance> resistances;

        public HitboxData(float baseDamageMultiplier)
        {
            this.baseDamageMultiplier = baseDamageMultiplier;
            vulnerabilities = new List<DamageVulnerability>();
            resistances = new List<DamageResistance>();
        }
    
    }
}