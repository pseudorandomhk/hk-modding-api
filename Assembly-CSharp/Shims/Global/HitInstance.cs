using System;
using UnityEngine;

#pragma warning disable CS1591 // suppress missing doc warning

/// <summary>
/// Port of <c>HitInstance</c>
/// </summary>
[Serializable]
public struct HitInstance
{
    public float GetActualDirection(Transform target)
    {
        if (Source != null && target != null && CircleDirection)
        {
            Vector2 vector = target.position - Source.transform.position;
            return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
        }
        return Direction;
    }

    public GameObject Source;
    public int AttackType;
    public bool CircleDirection;
    public int DamageDealt;
    public float Direction;
    public bool IgnoreInvulnerable;
    public float MagnitudeMultiplier;
    public float MoveAngle;
    public bool MoveDirection;
    public float Multiplier;
    public int SpecialType;
    public bool IsExtraDamage;
}
