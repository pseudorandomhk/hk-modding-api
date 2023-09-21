using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Port of <c>MaybeNullWhenAttribute</c>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class MaybeNullWhenAttribute : Attribute
{
    /// <summary>
    /// Gets the return value condition.
    /// </summary>
    public bool ReturnValue { get; private set; }

    /// <summary>
    /// Initializes the attribute with the specified return value condition.
    /// </summary>
    /// <param name="returnValue">The return value condition. If the method returns this value, the associated parameter may be <c>null</c>.</param>
    public MaybeNullWhenAttribute(bool returnValue)
    {
        this.ReturnValue = returnValue;
    }
}
