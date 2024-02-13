using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Data;
using Unity.Mathematics;

namespace H1M4W4R1.LUNA.Weapons.Jobs.Data
{
    /// <summary>
    /// Represents hit information to be used with Jobs
    /// </summary>
    public struct WeaponHitData
    {
        public WeaponData weaponData;
        public HitboxData hitboxData;

        public float3 weaponPos;
        public quaternion weaponRotation;

        public float3 hitPos;
        public float3 hitNormal;
    }
}