using System;
using System.Collections.Generic;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace H1M4W4R1.LUNA.Entities
{
    [Serializable]
    public struct HitboxData : INativeDisposable
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier;
        
        [RuntimeGenerated]
        public UnsafeList<DamageVulnerability> vulnerabilities;
        
        [RuntimeGenerated]
        public UnsafeList<DamageResistance> resistances;

        [NotBurstCompatible]
        public void RegisterVulnerabilities(List<DamageVulnerability> data)
        {
            vulnerabilities = new UnsafeList<DamageVulnerability>(data.Count, Allocator.Domain);
            foreach (var obj in data) vulnerabilities.Add(obj);
        }

        [NotBurstCompatible]
        public void RegisterResistances(List<DamageResistance> data)
        {
            resistances = new UnsafeList<DamageResistance>(data.Count, Allocator.Domain);
            foreach (var obj in data) resistances.Add(obj);
        }

        [BurstCompile]
        public void Dispose()
        {
            vulnerabilities.Dispose();
            resistances.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            // Dispose is not a crucial operation
            vulnerabilities.Dispose();
            resistances.Dispose();
            return inputDeps;
        }
    }
}