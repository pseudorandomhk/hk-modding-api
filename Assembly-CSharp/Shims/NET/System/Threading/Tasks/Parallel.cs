using System;
using System.Collections.Generic;

namespace System.Threading.Tasks;

// laziness ftw
/// <summary>
/// Interop implementation of <c>System::Threading::Tasks::Parallel</c> from .Net 4.0+.
/// No operations are actually done in parallel
/// </summary>
public static class Parallel
{
    /// <summary>
    /// Executes a for each operation on an <see cref="T:System.Collections.IEnumerable{TSource}"/> 
    /// in which iterations may run in parallel.
    /// </summary>
    /// <typeparam name="TSource">The type of the data in the source.</typeparam>
    /// <param name="source">An enumerable data source.</param>
    /// <param name="body">The delegate that is invoked once per iteration.</param>
    /// <remarks>
    /// The <paramref name="body"/> delegate is invoked once for each element in the <paramref name="source"/> 
    /// enumerable.  It is provided with the current element as a parameter.
    /// </remarks>
    public static void ForEach<TSource>(IEnumerable<TSource> source, Action<TSource> body)
    {
        foreach (var elem in source)
        {
            body(elem);
        }
    }

    /// <summary>
    /// Executes each of the provided actions, possibly in parallel.
    /// </summary>
    /// <param name="actions">An array of <see cref="T:System.Action">Actions</see> to execute.</param>
    /// <remarks>
    /// This method can be used to execute a set of operations, potentially in parallel.   
    /// No guarantees are made about the order in which the operations execute or whether 
    /// they execute in parallel.  This method does not return until each of the 
    /// provided operations has completed, regardless of whether completion 
    /// occurs due to normal or exceptional termination.
    /// </remarks>
    public static void Invoke(params Action[] actions)
    {
        foreach (var action in actions)
        {
            action();
        }
    }
}