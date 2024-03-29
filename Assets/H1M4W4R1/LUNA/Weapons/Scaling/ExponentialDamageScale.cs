﻿using Unity.Burst;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Scaling
{
    /// <summary>
    /// Represents exponential damage scaling.
    /// DMG = base_dmg ^ speedFactor
    /// </summary>
    [BurstCompile]
    public static class ExponentialDamageScale
    {
        [BurstCompile]
        public static float Calculate(in float baseDamage, in float speedMultiplier) =>
            math.pow(baseDamage, speedMultiplier);
    }
}