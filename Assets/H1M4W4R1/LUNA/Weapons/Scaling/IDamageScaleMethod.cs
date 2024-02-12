using Unity.Burst;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents damage scaling method for weapons.
    /// Damage scales against recent speed for specified weapon type.
    /// For example dynamic weapon speed is based on average speed in recent time and
    /// static weapon speed is always equal to one.
    /// </summary>
    public interface IDamageScaleMethod
    {
        [BurstCompile]
        public float CalculateScaleFrom(in float speedMultiplier);
        
        [BurstCompile]
        float GetBaseDamage();
    }
}