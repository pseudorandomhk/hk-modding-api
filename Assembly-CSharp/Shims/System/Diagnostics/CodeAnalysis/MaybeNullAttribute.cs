namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Port of <c>MaybeNullAttribute</c>
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
public sealed class MaybeNullAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MaybeNullAttribute"/> class.
    /// </summary>
    public MaybeNullAttribute() { }
}
