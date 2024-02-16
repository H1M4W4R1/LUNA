using Unity.Burst;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents flat damage scaling.
    /// DMG = base_dmg
    /// </summary>
    [BurstCompile]
    public static class FlatDamageScale
    {
        [BurstCompile]
        public static float Calculate(in float baseDamage, in float speedMultiplier) => baseDamage;
    }
}