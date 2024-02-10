# Pre
Package is still Work In Progress. (Feel free to improve it and drop a PR)

# LUNA - Legendary Unity Nifty Arsenal
LUNA is an Asset Package dedicated to creation of weapons. Primarly it was focused on medieval-style weapons like swords or arrows, however I decided to also consider magic and guns (because why not).

It also has a advantage of being able to differentiate attack types within a single weapon (eg. piercing and slashing attack for sword based on velocity - spike of sword does pierce while blade does slash). This can be a useful feature for immersive VR gaming where you can beat different types of armor using same weapon.

# Requirements
* Unity.Mathematics
* Unity.Burst
* Unity.Collections

# Weapons
## Static wapon (Component)
This is a weapon that is placed in world. It can be for example a spike in a spike trap (that pit Indiana Jones never has fallen into).
## Dynamic weapon (Component)
This type represents a weapon that can be wielded by user. Weapon damage is computed dynamically based on current weapon swing energy. The weapon energy / speed factor is defined by below formula:

```
Speed Factor is equal to modulus of moving average of velocity across specified timespan (expected attack time)
```

In other words: the system is resistant to things like quick wrist attacks while wielding a greatsword - as you need to smash it for a good second to gain full power.

## Custom weapon types
You can also implement custom weapon types using WeaponBase abstract class.

# Damage scaling
## Speed
Also it is worth noting that damage for a dynamic weapon (for static weapon speed factor is always 1) can scale via different methods:

### Flat damage scaling
Damage will always be equal to base damage. Simple as that. (Sometimes may be useful)

### Linear damage scaling
Damage is equal to base damage multiplied by speed factor. Eg. if speed factor is 20 and base damage is 10 then result is 10*20 = 200 DMG.

### Power damage scaling
Damage is equal to base damage multiplied by speed factor to nTh power. Eg if speed factor is 10 and base damage is 10, power is 2 then result is 10*10^2 = 1000 DMG.

### Exponential damage scaling
Damage is scaled exponentially - base damage is brought to power of speed factor eg. base damage is 10 and speed factor is 5 then damage is 10e5 DMG. This is good for nuclear blast :D

## Vulnerability, Resistance and damage types
System supports multiple damage types including melee (slashing, piercing...), magic (fire, ice...) or other (poison, silver...)

Hitbox can have vulnerability and resistance to specified damage which leads to damage being multiplied or divided by specified factor. Of course hitbox also supports multiple types of vulnerability scaling (if multiple vulnerabilities exist):

* **None**: mult = max(mult1, mult2, mult3, ...)
* **Additive**: mult = (mult1 + mult2 + mult3 + ...)
* **Multiplicative**: mult = (mult1 * mult2 * mult3 * ...)
* **Exponential**: mult = (mult1 ^ mult2 ^ mult3 ^ ...), computationally expensive

Also weapon has different attack vectors which can lead to different types of damage types - eg. spike of sword is a Piercing attack, however blade is slashing. Hitting enemy with flat side of sword can be impact attack. In most games this thing is ignored.

Damage type of attack is always a logic sum of vector damage types and weapon damage types - if you want to make a fire sword, then you assign vector damage types to melee subtypes (like mentioned above) and weapon damage type to fire. Simple as that.

# Hitboxes
Enemy will have hitboxes that can have damage dealt to. Hitbox itself also has damage multiplier (default: 1) to support things like headshots or weaken hits to arms / legs.

Hitbox is directly related to vulnerabilities.

# Weapon damage dealing subsystems
Weapons can have different damage dealing subsystems eg. on collision or on trigger enter. Those subsystems are separate MonoBehaviours. You should have at least one on your weapon to work properly.

Types of damage systems:
* Collision
* Trigger (you can scale trigger to simulate explosion)

# Planned features
* Demo scene
* In-Editor Weapon Editor
* Custom Inspectors
