using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Jobs.Data
{
    /// <summary>
    /// Represents movement data to be used with Job system
    /// </summary>
    public struct WeaponMovementData
    {
        public WeaponData weaponData;
        public float3 position;
        public float3 previousPosition;
        public float deltaTime;
    }
}