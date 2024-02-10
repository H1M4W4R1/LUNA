using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons
{
    /// <summary>
    /// Represents static in-world weapon eg. pit spike trap
    /// </summary>
    [BurstCompile]
    public class StaticWeapon : WeaponBase
    {
        [BurstCompile]
        public override float3 GetRecentSpeed() => new float3(1, 1, 1);

        [BurstCompile]
        public override float GetSpeedDamageMultiplier() => _damageScaleMethod.CalculateScaleFrom(1f);

    }
}