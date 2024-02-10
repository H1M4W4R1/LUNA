using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents power damage scaling.
    /// DMG = base_dmg * speedFactor ^ power
    /// </summary>
    [BurstCompile]
    public struct PowerDamageScale : IDamageScaleMethod
    {
        public float baseDamage;
        public int power;
        
        [BurstCompile]
        public float CalculateScaleFrom(in float speedMultiplier)
        {
            return math.pow(speedMultiplier, power) * baseDamage;
        }
        
        public float GetBaseDamage() => baseDamage;
    }
}