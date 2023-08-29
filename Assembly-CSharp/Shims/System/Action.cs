namespace Shims.NET.System;

/// <summary>
/// Contravariant version of System::Action&lt;T&gt;, following its definition in .NET 4.0+
/// </summary>
/// <typeparam name="T">Parameter type</typeparam>
public delegate void Action<in T>(T obj);