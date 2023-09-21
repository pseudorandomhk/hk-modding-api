using System;
using UnityEngine;
using MonoMod;

#pragma warning disable 1591

public static class Extensions
{
    public static void SetActiveChildren(this GameObject self, bool value)
    {
        int childCount = self.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            self.transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}
