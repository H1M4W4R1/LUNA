using System;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Scaling;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons
{
    /// <summary>
    /// Represents base weapon information and abstractions
    /// </summary>
    [BurstCompile]
    public abstract class WeaponBase : MonoBehaviour
    {
        private IDamageScaleMethod _damageScaleMethod;

        [Tooltip("Type of damage scaling for this weapon")]
        public DamageScaleMethod damageScaleMethod;
        
        [Tooltip("Flat damage of this weapon - aka. base damage")]
        public float flatDamage;
        
        [Tooltip("Power of this weapon, used only in case of DamageScaleMethod.Power")]
        public int damagePower;

        [Tooltip("Weapon vulnerability scaling")]
        public VulnerabilityScaling vulnerabilityScaling;
        
        /// <summary>
        /// Get current speed of this weapon.
        /// </summary>
        public abstract float3 GetRecentSpeed();

        public abstract float GetSpeedDamageMultiplier();

        /// <summary>
        /// Initialize weapon data
        /// </summary>
        private void Initialize()
        {
            // Prepare damage scale system
            switch (damageScaleMethod)
            {
                case DamageScaleMethod.Flat:
                    _damageScaleMethod = new FlatDamageScale() {baseDamage = flatDamage};
                    break;
                case DamageScaleMethod.Linear:
                    _damageScaleMethod = new LinearDamageScale() {baseDamage = flatDamage};
                    break;
                case DamageScaleMethod.Power:
                    _damageScaleMethod = new PowerDamageScale() {baseDamage = flatDamage, power = damagePower};
                    break;
                case DamageScaleMethod.Exponential:
                    _damageScaleMethod = new ExponentialDamageScale() {baseDamage = flatDamage};
                    break;
            }
        }

        protected void Awake()
        {
            Initialize();
        }
    }
}
