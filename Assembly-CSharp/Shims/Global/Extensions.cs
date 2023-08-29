using UnityEngine;

namespace Shims;

#pragma warning disable CS1591
#pragma warning disable CS0472 // copied code from dnSpy

/// <summary>
/// Port of later additions to <see cref="Extensions"/>
/// </summary>
public static class Extensions
{
    public static bool HasParameter(this Animator self, string paramName, AnimatorControllerParameterType? type = null)
    {
        foreach (AnimatorControllerParameter param in self.parameters)
        {
            if (param.name == paramName && (type != null || (param.type != null && param.type == type.GetValueOrDefault())))
            {
                return true;
            }
        }
        return false;
    }

    public static void SetRotation2D(this Transform t, float rotation)
    {
        Vector3 eulerAngles = t.eulerAngles;
        eulerAngles.z = rotation;
        t.eulerAngles = eulerAngles;
    }
}
