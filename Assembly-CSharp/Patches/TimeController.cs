using System;
using UnityEngine;
using MonoMod;

#pragma warning disable 1591

public static class TimeController
{
    private static float slowMotionTimeScale = 1f;
    private static float pauseTimeScale = 1f;
    private static float platformBackgroundTimesScale = 1f;
    private static float genericTimeScale = 1f;

    public static float SlowMotionTimeScale
    {
        get
        {
            return slowMotionTimeScale;
        }

        set
        {
            SetTimeScaleFactor(ref slowMotionTimeScale, value);
        }
    }

    public static float PauseTimeScale
    {
        get
        {
            return pauseTimeScale;
        }

        set
        {
            SetTimeScaleFactor(ref pauseTimeScale, value);
        }
    }

    public static float PlatformBackgroundTimeScale
    {
        get
        {
            return platformBackgroundTimesScale;
        }

        set
        {
            SetTimeScaleFactor(ref platformBackgroundTimesScale, value);
        }
    }

    public static float GenericTimeScale
    {
        get
        {
            return genericTimeScale;
        }

        set
        {
            SetTimeScaleFactor(ref genericTimeScale, value);
        }
    }

    private static void SetTimeScaleFactor(ref float field, float value)
    {
        if (field != value)
        {
            field = value;
            float num = slowMotionTimeScale * pauseTimeScale * platformBackgroundTimesScale * genericTimeScale;
            if (num < 0.01f)
            {
                num = 0;
            }
            Time.timeScale = num;
        }
    }
}
