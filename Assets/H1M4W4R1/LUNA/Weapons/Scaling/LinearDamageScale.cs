using Unity.Burst;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents linear damage scaling.
    /// DMG = base_dmg * speedFactor
    /// </summary>
    [BurstCompile]
    public static class LinearDamageScale
    {
        [BurstCompile]
        public static float Calculate(in float baseDamage, in float speedMultiplier) => 
            baseDamage * speedMultiplier;
    }
}