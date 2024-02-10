using System;

namespace H1M4W4R1.LUNA.Weapons.Damage
{
    /// <summary>
    /// Represents common RPG damage types.
    /// </summary>
    [Flags]
    public enum DamageType
    {
        None =              0x00000000,
        
        // Melee damage types
        Melee =             0x10000000,
        Slashing =  Melee | 0x00000002, // Regular sword edge
        Piercing =  Melee | 0x00000004, // Spikes
        Impact =    Melee | 0x00000008, // If nothing works - use a hammer
        
        // Magic damage types
        Magic =             0x20000000, // Basic magic damage
        Fire =      Magic | 0x00000010, // FIREBALLS!
        Cold =      Magic | 0x00000020, // Let's make them fragile
        Lightning = Magic | 0x00000040, // Just don't stand in water
        Necrotic =  Magic | 0x00000080, // I need a helping hand
        Psychic =   Magic | 0x00000100, // Your mind to my mind, your thoughts to my thoughts
        
        // Semi-magic religious damage types
        Holy =      Magic | 0x00000200, // for holy weapons
        Unholy =    Magic | 0x00000400, // for unholy weapons
        
        // Another damage types :D
        Other =             0x40000000,
        Poison =    Other | 0x00000800, // Like my GF
        Acid =      Other | 0x00001000,
        Radiant =   Other | 0x00002000,
        Silver =    Other | 0x00004000, // for monsters vulnerable to silver weapons
        Unarmed =   Other | 0x00008000, // for characters without a specific weapon
        Explosive = Other | 0x00010000// for explosive devices or ammunition
        
        // TL;DR: Add more here or to specified groups ;)
    }
}