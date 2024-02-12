using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Scaling;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Data
{
    [Serializable]
    public struct WeaponData : INativeDisposable
    {
        [Tooltip("Type of damage scaling for this weapon")]
        public DamageScaleMethod damageScaleMethod;
        
        [Tooltip("Flat damage of this weapon - aka. base damage")]
        public float flatDamage;

        [Tooltip("Weapon vulnerability scaling")]
        public VulnerabilityScaling vulnerabilityScaling;

        [Tooltip("Weapon damage type, applies this damage type to all damage vectors")]
        public DamageType damageType;

        [Tooltip("Distance weight while computing nearest vector")]
        public float distanceWeight;
        
        [Tooltip("Angle weight while computing nearest vector")]
        public float angleWeight;

        [Tooltip("If false then opposing vectors will be considered to have same angle (angle will be computed in [0,π] range)")]
        [MarshalAs(UnmanagedType.U1)]
        public bool directionMatters;

        [Tooltip("How long should it take to swing this weapon")]
        public float expectedAttackTime;

        [RuntimeGenerated]
        public UnsafeList<WeaponDamageVector> damageVectors;

        [RuntimeGenerated]
        public float3 currentSpeed;
        
        [RuntimeGenerated]
        public float speedDamageMultiplier;

        [NotBurstCompatible]
        public void RegisterVectors(List<WeaponDamageVector> vectors)
        {
            damageVectors = new UnsafeList<WeaponDamageVector>(vectors.Count, Allocator.Domain);
            foreach (var vector in vectors)
                damageVectors.Add(vector);
        }

        [BurstCompile]
        public float GetSpeedDamageMultiplier() => speedDamageMultiplier;
        
        [BurstCompile]
        public void Dispose() => damageVectors.Dispose();
        
        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps) => damageVectors.Dispose(inputDeps);

    }
}