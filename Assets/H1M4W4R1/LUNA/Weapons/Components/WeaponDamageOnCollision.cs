using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Components
{
    /// <summary>
    /// Deals weapon damage when weapon collides with damageable object
    /// </summary>
    [RequireComponent(typeof(WeaponBase))]
    public class WeaponDamageOnCollision : WeaponDamageSystemBase
    {
        private void OnCollisionEnter(Collision other)
        {
            // Get HitBox component
            var hitbox = other.gameObject.GetComponent<Hitbox>();

            // Only first contact matters
            var cPoint = other.GetContact(0);

            // Deal damage
            Process(hitbox, cPoint.point, cPoint.normal);
        }
    }
}