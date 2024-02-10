using Unity.Burst;

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
        public float CalculateScaleFrom(in float speedMultiplier) => baseDamage * speedMultiplier;
        
        public float GetBaseDamage() => baseDamage;
    }
}