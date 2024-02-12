using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Burst
{
    [BurstCompile] [BurstCompatible]
    public static class WeaponCalculation
    {
        [BurstCompile] [BurstCompatible]
        public static void UpdateWeaponSpeed(ref WeaponData data, in float3 currentSpeed, in float deltaTime)
        {
            
            // Moving average formula using LERP 
            var weight = math.clamp(deltaTime / data.expectedAttackTime, 0f, 1f);
            data.currentSpeed = math.lerp(data.currentSpeed, currentSpeed, weight);
        }
    }
}