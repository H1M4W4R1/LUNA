using Unity.Burst;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents flat damage scaling.
    /// DMG = base_dmg
    /// </summary>
    [BurstCompile]
    public struct FlatDamageScale : IDamageScaleMethod
    {
        public float baseDamage;
        
        [BurstCompile]
        public float CalculateScaleFrom(float speedMultiplier)
        {
            return baseDamage;
        }
    }
}