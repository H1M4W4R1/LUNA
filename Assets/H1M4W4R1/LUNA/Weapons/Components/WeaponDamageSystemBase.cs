using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Burst;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Components
{
    /// <summary>
    /// Represents basic weapon subsystem that is responsible for detecting damage occurence.
    /// It can be for example: when weapon collides with enemy or when enemy enters trigger.
    /// </summary>
    [BurstCompile]
    public abstract class WeaponDamageSystemBase : MonoBehaviour
    {
        private WeaponBase _weapon;
        protected Transform _transform;
        
        protected void Awake()
        {
            _weapon = GetComponent<WeaponBase>();
            _transform = transform;
        }

        protected DamageInfo Process(HitboxData hitbox, float3 pos, quaternion rot, float3 hitPos, float3 hitNormal)
        {
            ProcessWeaponHitJob.Prepare(_weapon.GetData(), hitbox, pos, rot, hitPos, hitNormal,
                out var job);
            job.Schedule().Complete();
            
            // Copy value of damage info
            var dmgInfo = job.GetDamageInfo();
            job.Dispose();
            return dmgInfo;
        }
    }
}