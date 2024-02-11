using System.Collections.Generic;
using H1M4W4R1.LUNA.Weapons.Computation;
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
        // NOTE: This is bad, but I don't care. I need this field to be shared between Tool and Editor
        // And accessing one from another is even worse cause it comes to reflection
        #if UNITY_EDITOR
        public int selectedIndex;
        #endif
        
        protected IDamageScaleMethod _damageScaleMethod; // Internal structural dependency

        [Tooltip("Type of damage scaling for this weapon")]
        public DamageScaleMethod damageScaleMethod;
        
        [Tooltip("Flat damage of this weapon - aka. base damage")]
        public float flatDamage;

        [Tooltip("Weapon vulnerability scaling")]
        public VulnerabilityScaling vulnerabilityScaling = VulnerabilityScaling.Multiplicative;

        [Tooltip("Weapon damage type, applies this damage type to all damage vectors")]
        public DamageType damageType = DamageType.None;

        public float distanceWeight = 1f;
        public float angleWeight = 5f;

        [Tooltip("If false then opposing vectors will be considered to have same angle (angle will be computed in [0,π] range)")]
        public bool directionMatters = true;
        
        /// <summary>
        /// List of vectors used in damage computation - nearest vector is acquired (also taking angle into consideration)
        /// Then damage is dealt based on damage type of that vector
        /// </summary>
        [SerializeField]
        private List<WeaponDamageVector> damageVectors = new List<WeaponDamageVector>();

        public bool HasVectors => damageVectors.Count > 0;

        public List<WeaponDamageVector> GetVectors() => damageVectors;

        public void AddVector(ref WeaponDamageVector vector) => damageVectors.Add(vector);

        /// <summary>
        /// Get current speed of this weapon.
        /// </summary>
        public abstract float3 GetRecentSpeed();

       [BurstCompile]
        public abstract float GetSpeedDamageMultiplier();

        public float GetBaseDamage() => _damageScaleMethod.GetBaseDamage();

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
            Initialize();
        }

        /// <summary>
        /// Find closest damage vector based on attack point and normalized direction
        /// </summary>
        [BurstCompile]
        public WeaponDamageVector FindClosestDamageVector(in float3 collisionPoint,
            in float3 collisionNormal)
        {
            var tObj = transform;

            return FindClosestDamageVector(tObj.position, tObj.rotation, collisionPoint, collisionNormal);
        }
        
        /// <summary>
        /// Find closest damage vector based on attack point and normalized direction
        /// </summary>
        [BurstCompile]
        private WeaponDamageVector FindClosestDamageVector(
            in float3 position,
            in quaternion rotation,
            in float3 collisionPoint,
            in float3 collisionNormal)
        {
            if(damageVectors.Count < 1)
                Debug.LogError("[LUNA] Weapon must have at least one damage vector. Otherwise it's useless!");

            // Invert collision normal for calculation (use anti-normal)
            var cAntiNormal = -collisionNormal;
            
            var closestStruct = default(WeaponDamageVector);
            var minScore = float.MaxValue;

            // INFO: NVM forgot that Burst does not compile from managed objects.
            foreach (var currentStruct in damageVectors)
            {
                // Calculate angle difference (cosine similarity between **normalized** vectors)
                var angleDifference = 
                    math.dot(currentStruct.GetVectorForRotation(rotation), cAntiNormal);

                // Ignore direction if does not matter
                if (!directionMatters)
                    angleDifference = math.abs(angleDifference);

                // Calculate distance (startPoint is in local space, collision is in world space)
                var distance = math.distance(currentStruct.startPoint + position, collisionPoint);

                // Combine angle and distance using specified weights
                var score = angleWeight * (1 - angleDifference) + distanceWeight * distance;

                // Update closest struct if the current score is smaller
                if (score < minScore)
                {
                    minScore = score;
                    closestStruct = currentStruct;
                }
            }
            
            return closestStruct;
        }

    }
}
