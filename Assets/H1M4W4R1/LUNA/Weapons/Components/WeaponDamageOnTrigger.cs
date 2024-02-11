using H1M4W4R1.LUNA.Entities;
using H1M4W4R1.LUNA.Weapons.Damage;
using Unity.Mathematics;
using UnityEngine;

namespace H1M4W4R1.LUNA.Weapons.Components
{
    // BUG: this class causes crash on Play-Stop when MonoBehaviour is selected and inspector is not visible all time
    /// <summary>
    /// Deals weapon damage when weapon triggers with damageable object.
    /// Use <see cref="WeaponDamageOnCollision"/> when possible.
    /// This is okay for explosions and any other non-directional attacks (fireballs too).
    /// </summary>
    [RequireComponent(typeof(WeaponBase))]
    public class WeaponDamageOnTrigger : WeaponDamageSystemBase
    {
        private void OnTriggerEnter(Collider other)
        {
            // Get components
            var hitbox = other.gameObject.GetComponent<Hitbox>();
            var hTransform = hitbox.transform;
            var hitboxPosition = hTransform.position;

            // Compute direction (somewhat okay-ish)
            var direction = math.normalize(hitboxPosition - _transform.position);
            
            // Deal damage
            Process(hitbox.data, hitboxPosition, direction, out var dmgInfo);
            hitbox.DealDamage(ref dmgInfo);
        }
    }
}