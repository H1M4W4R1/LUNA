using H1M4W4R1.LUNA.Weapons;
using UnityEngine;
using UnityEngine.Events;

namespace H1M4W4R1.LUNA.Entities
{
    public class Hitbox : MonoBehaviour
    {
        [Tooltip("Damage will be multiplied if this hitbox is hit")]
        public float baseDamageMultiplier = 1f;

        /// <summary>
        /// Executed when this hitbox gets hit with a weapon
        /// </summary> 
        public UnityEvent<DamageInfo> onHit;

        /// <summary>
        /// Deal damage to this hitbox
        /// </summary>
        public virtual void DealDamage(DamageInfo info)
        {
            onHit.Invoke(info);
        }
    }
}