using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents exponential damage scaling.
    /// DMG = base_dmg ^ speedFactor
    /// </summary>
    [BurstCompile]
    public struct ExponentialDamageScale : IDamageScaleMethod
    {
        public float baseDamage;

        [BurstCompile]
        public float CalculateScaleFrom(float speedMultiplier)
        {
            return math.pow(baseDamage, speedMultiplier);
        }

        public float GetBaseDamage() => baseDamage;
    }
}