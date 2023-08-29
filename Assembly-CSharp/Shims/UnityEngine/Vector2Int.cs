using System;

namespace UnityEngine;

#pragma warning disable CS1591 // suppress missing doc warning

/// <summary>
/// Port of <c>Vector2Int</c> from later versions of Unity
/// </summary>
public struct Vector2Int
{
    public int x { get; set; }
    public int y { get; set; }
    public int this[int index]
    {
        get
        {
            if (index == 0) return x;
            else if (index == 1) return y;
            else throw new IndexOutOfRangeException();
        }

        set
        {
            if (index == 0) x = value;
            else if (index == 1) y = value;
            else throw new IndexOutOfRangeException();
        }
    }
    public int sqrMagnitude => x * x + y * y;
    public float magnitude => Mathf.Sqrt(sqrMagnitude);

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override bool Equals(object other)
    {
        if (other == null || this.GetType() != other.GetType()) return false;
        Vector2Int otherVector2Int = (Vector2Int)other;
        return x == otherVector2Int.x && y == otherVector2Int.y;
    }

    public override int GetHashCode()
    {
        return ((x << 16) | (x >>> 16)) ^ y;
    }

    public static bool operator ==(Vector2Int lhs, Vector2Int rhs) => lhs.Equals(rhs);

    public static bool operator !=(Vector2Int lhs, Vector2Int rhs) => !lhs.Equals(rhs);

    public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs) => new Vector2Int(rhs.x + lhs.x, rhs.y + lhs.y);

    public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs) => new Vector2Int(lhs.x - rhs.x, lhs.y - rhs.y);

    public static Vector2Int operator *(Vector2Int a, Vector2Int b) => new Vector2Int(a.x * b.x, a.y * b.y);

    public static Vector2Int operator *(Vector2Int a, int b) => new Vector2Int(a.x * b, a.y * b);

    public static Vector2Int operator /(Vector2Int a, int b) => new Vector2Int(a.x / b, a.y / b);

    public static implicit operator Vector2(Vector2Int v) => new Vector2(v.x, v.y);
}
