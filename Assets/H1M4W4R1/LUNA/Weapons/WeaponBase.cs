using System.Collections.Generic;
using H1M4W4R1.LUNA.Attributes;
using H1M4W4R1.LUNA.Weapons.Computation;
using H1M4W4R1.LUNA.Weapons.Damage;
using H1M4W4R1.LUNA.Weapons.Data;
using H1M4W4R1.LUNA.Weapons.Scaling;
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
        
        [SerializeField]
        [FormerlySerializedAs("_weaponData")] protected WeaponData weaponData = new WeaponData()
        {
            damageScaleMethod = DamageScaleMethod.Linear,
            damageType = DamageType.None,
            flatDamage = 10,
            directionMatters = true,
            vulnerabilityScaling = VulnerabilityScaling.Multiplicative,
            distanceWeight = 1,
            angleWeight = 5,
            expectedAttackTime = 0.1f
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

        public WeaponData GetData() => weaponData;

        protected void Awake()
        {
            // Prepare weapon vectors
            weaponData.RegisterVectors(damageVectors);
            weaponData.weaponScale = transform.lossyScale;
        }

        private void OnDestroy()
        {
            weaponData.Dispose();
        }
    }
}
