namespace System;

/// <summary>
/// Helper for implementing GetHashCode()
/// </summary>
public static class HashCode
{
    // idk, just took this from dnSpy
    private const int HASH_POWER = -1521134295;

    /// <summary>
    /// Combine the given <paramref name="hashCodes"/> into a single hash code.
    /// </summary>
    /// <param name="hashCodes">The hash codes to combine.</param>
    /// <returns>The combination of all hash codes in <paramref name="hashCodes"/>.</returns>
    public static int Combine(params int?[] hashCodes)
    {
        int hash = 0;
        unchecked // don't check for overflow
        {
            foreach (int? hashCode in hashCodes)
                hash = (hash * HASH_POWER) + (hashCode ?? 0);
        }
        return hash;
    }
}