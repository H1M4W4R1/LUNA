﻿using System;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Computation
{
    /// <summary>
    /// Represents vector of weapon that can deal damage.
    /// Example vectors can be:
    /// * blade side - which should deal slash damage
    /// * blade tip - which should deal piercing damage
    /// </summary>
    [Serializable]
    public struct WeaponDamageVector
    {
        /// <summary>
        /// Start point of vector in object local space
        /// </summary>
        public float3 startPoint;
        
        /// <summary>
        /// Normalized directional vector for damage computation
        /// </summary>
        public float3 relativeVectorNormalized;

        /// <summary>
        /// Computes relative vector based by specified rotation of object
        /// </summary>
        public float3 GetVectorForRotation(quaternion objectRotation) => math.rotate(objectRotation, relativeVectorNormalized);

        /// <summary>
        /// Vector damage type
        /// </summary>
        public DamageType damageType;
    }
}