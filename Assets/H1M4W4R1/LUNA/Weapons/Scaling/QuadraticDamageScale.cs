using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents power damage scaling.
    /// DMG = base_dmg * speedFactor ^ 2
    /// </summary>
    [BurstCompile]
    public static class QuadraticDamageScale
    {
        [BurstCompile]
        public static float Calculate(in float baseDamage, in float speedMultiplier) =>
            math.pow(speedMultiplier, 2) * baseDamage;
    }
}