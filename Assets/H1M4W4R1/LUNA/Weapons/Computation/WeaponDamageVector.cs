using System;
using System.Runtime.InteropServices;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine.Serialization;

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

        [BurstCompile]
        public float3 GetStartPoint(quaternion objectRotation, float3 weaponScale) => math.rotate(objectRotation, startPoint) * weaponScale;

        public void SetStartPoint(quaternion objectRotation, float3 weaponScale, float3 point)
            => startPoint = math.rotate(math.inverse(objectRotation), point) / weaponScale;  
        
        /// <summary>
        /// Vector damage type
        /// </summary>
        public DamageType damageType;
        
        #region RUNTIME_VARIABLES

        [RuntimeGenerated] [MarshalAs(UnmanagedType.U1)]
        public bool posIsNotNull;
        
        [RuntimeGenerated]
        public float3 previousPosition;

        [RuntimeGenerated]
        public float3 currentPosition;
        
        [RuntimeGenerated]
        public float3 currentVelocity;
        
        [RuntimeGenerated]
        public float currentSpeed;

        [RuntimeGenerated]
        public float currentBaseDamage;
        
        #endregion

    }
}