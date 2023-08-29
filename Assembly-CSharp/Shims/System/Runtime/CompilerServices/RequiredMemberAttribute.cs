namespace System.Runtime.CompilerServices;

/// <summary>
/// Specifies that a type has required members or that a member is required.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct,
    AllowMultiple = false, Inherited = false)]
public sealed class RequiredMemberAttribute : Attribute
{
    /// <summary>
    /// Initializes a <see cref="RequiredMemberAttribute"/> instance with default values.
    /// </summary>
    public RequiredMemberAttribute() { }
}
