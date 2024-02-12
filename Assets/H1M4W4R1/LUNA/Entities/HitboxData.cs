using System;
using System.Collections.Generic;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace H1M4W4R1.LUNA.Entities
{
    [Serializable]
    public struct HitboxData : INativeDisposable
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier;
        
        public NativeList<DamageVulnerability> vulnerabilities;
        public NativeList<DamageResistance> resistances;

        [NotBurstCompatible]
        public void RegisterVulnerabilities(List<DamageVulnerability> data) =>
            vulnerabilities = new NativeList<DamageVulnerability>(data.Count, Allocator.Domain);
        
        [NotBurstCompatible]
        public void RegisterResistances(List<DamageResistance> data) =>
            resistances = new NativeList<DamageResistance>(data.Count, Allocator.Domain);

        public void Dispose()
        {
            vulnerabilities.Dispose();
            resistances.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps) =>
            vulnerabilities.Dispose(resistances.Dispose(inputDeps));
    }
}