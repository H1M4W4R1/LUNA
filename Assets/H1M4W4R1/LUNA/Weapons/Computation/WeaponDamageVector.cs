using System;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Computation
{
    /// <summary>
    /// Represents vector of weapon that can deal damage.
    /// Example vectors can be:
    /// * blade side - which should deal slash damage
    /// * blade tip - which should deal piercing damage
    /// </summary>
    [Serializable] [BurstCompile]
    public struct WeaponDamageVector
    {
        /// <summary>
        /// Start point of vector in object local space
        /// </summary>
        public float3 startPoint;
        
        /// <summary>
        /// Normalized directional vector for damage computation
        /// </summary>
        public quaternion vectorRotation;

        /// <summary>
        /// Computes relative vector based by specified rotation of object
        /// </summary>
       [BurstCompile]
        public float3 GetVectorForRotation(quaternion objectRotation) => 
            math.rotate(objectRotation, math.rotate(vectorRotation, new float3(0, 0, 1)));

        /// <summary>
        /// Vector damage type
        /// </summary>
        public DamageType damageType;
    }
}