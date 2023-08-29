using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Collections.Concurrent;

internal static class LockingHelper
{
    public static void LockAndForwardCall(object _lock, Action call)
    {
        lock (_lock)
            call();
    }

    public static void LockAndForwardCall<T>(object _lock, Action<T> call, T arg1)
    {
        lock (_lock)
            call(arg1);
    }

    public static void LockAndForwardCall<TArg1, TArg2>(object _lock, Action<TArg1, TArg2> call, TArg1 arg1, TArg2 arg2)
    {
        lock (_lock)
            call(arg1, arg2);
    }

    public static T LockAndForwardCall<T>(object _lock, Func<T> call)
    {
        lock (_lock)
            return call();
    }

    public static TRet LockAndForwardCall<TRet, TArg1>(object _lock, Func<TArg1, TRet> call, TArg1 arg1)
    {
        lock (_lock)
            return call(arg1);
    }

    public static TRet LockAndForwardCall<TRet, TArg1, TArg2>(object _lock, Func<TArg1, TArg2, TRet> call, TArg1 arg1, TArg2 arg2)
    {
        lock (_lock)
            return call(arg1, arg2);
    }
}
