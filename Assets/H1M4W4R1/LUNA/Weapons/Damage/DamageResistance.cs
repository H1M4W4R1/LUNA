using System;
using Unity.Burst;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Damage
{
    /// <summary>
    /// Represents that current object is resistant to specified damage
    /// Can use multiple resistances scripts to define different vulnerability levels
    /// If weapon uses multiple damage types object is resistant to eg. unholy and fire against demon
    /// then damageMultiplier will take effect for each of damage type
    /// </summary>
    [BurstCompile]
    [Serializable]
    public struct DamageResistance
    {
        public DamageType typeOfDamage;
        
        [Tooltip("Damage will be divided by this level if resistant.")]
        public float damageAntiMultiplier;

        [BurstCompile]
        public bool IsResistantTo(in DamageType damageType)
        {
            return (damageType & typeOfDamage) > 0;
        }
    }
}