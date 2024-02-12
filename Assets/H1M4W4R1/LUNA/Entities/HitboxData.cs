using System;
using System.Collections.Generic;
using H1M4W4R1.LUNA.Weapons.Damage;
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
        
        public UnsafeList<DamageVulnerability> vulnerabilities;
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

        public void Dispose()
        {
            vulnerabilities.Dispose();
            resistances.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps) =>
            vulnerabilities.Dispose(resistances.Dispose(inputDeps));
    }
}