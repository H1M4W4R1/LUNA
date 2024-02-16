using System;

namespace H1M4W4R1.LUNA.Attributes
{
    /// <summary>
    /// Represents that variable is being updated in real-time during gameplay
    /// (mostly using jobs)
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class RuntimeGeneratedAttribute : Attribute
    {
        
    }
}