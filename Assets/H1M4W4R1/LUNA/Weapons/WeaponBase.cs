using System;
using System.Collections.Generic;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Scaling;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace H1M4W4R1.LUNA.Weapons
{
    /// <summary>
    /// Represents base weapon information and abstractions
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        // NOTE: This is bad, but I don't care. I need this field to be shared between Tool and Editor
        // And accessing one from another is even worse cause it comes to reflection
        #if UNITY_EDITOR
        public int selectedIndex;
        #endif
        
        protected IDamageScaleMethod _damageScaleMethod; // Internal structural dependency

        [FormerlySerializedAs("_weaponData")] public WeaponData weaponData = new WeaponData()
        {
            damageScaleMethod = DamageScaleMethod.Linear,
            damageType = DamageType.None,
            flatDamage = 10,
            directionMatters = true,
            vulnerabilityScaling = VulnerabilityScaling.Multiplicative
        };
      
        /// <summary>
        /// List of vectors used in damage computation - nearest vector is acquired (also taking angle into consideration)
        /// Then damage is dealt based on damage type of that vector
        /// DO NOT USE IN SCRIPTS. EDITOR-ONLY.
        /// </summary>
        [SerializeField] [EditorOnly]
        private List<WeaponDamageVector> damageVectors = new List<WeaponDamageVector>();

        [EditorOnly]
        public List<WeaponDamageVector> GetVectors() => damageVectors;
 
        /// <summary>
        /// Get current speed of this weapon.
        /// </summary>
        public abstract float3 GetRecentSpeed();

        public abstract float GetSpeedDamageMultiplier();

        public WeaponData GetData() => weaponData;
        
        /// <summary>
        /// Initialize weapon data
        /// </summary>
        private void Initialize()
        {
            var flatDamage = weaponData.flatDamage;
            
            // Prepare damage scale system
            switch (weaponData.damageScaleMethod)
            {
                case DamageScaleMethod.Flat:
                    _damageScaleMethod = new FlatDamageScale() {baseDamage = flatDamage};
                    break;
                case DamageScaleMethod.Linear:
                    _damageScaleMethod = new LinearDamageScale() {baseDamage = flatDamage};
                    break;
                case DamageScaleMethod.Quadratic:
                    _damageScaleMethod = new PowerDamageScale() {baseDamage = flatDamage};
                    break;
                case DamageScaleMethod.Exponential:
                    _damageScaleMethod = new ExponentialDamageScale() {baseDamage = flatDamage};
                    break;
            }
        }

        protected void Awake()
        {
            // Prepare weapon vectors
            weaponData.RegisterVectors(damageVectors);
            Initialize();
        }

        protected void Update()
        {
            // Update speed information
            weaponData.speedDamageMultiplier = GetSpeedDamageMultiplier();
        }

        private void OnDestroy()
        {
            weaponData.Dispose();
        }
    }
}
