using Unity.Burst;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents linear damage scaling.
    /// DMG = base_dmg * speedFactor
    /// </summary>
    [BurstCompile]
    public struct LinearDamageScale : IDamageScaleMethod
    {
        public float baseDamage;

        [BurstCompile]
        public float CalculateScaleFrom(float speedMultiplier) => baseDamage * speedMultiplier;
    }
}