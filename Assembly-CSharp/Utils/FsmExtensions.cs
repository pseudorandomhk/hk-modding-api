using HutongGames.PlayMaker;
using System.Collections.Generic;
using System.Linq;

namespace Modding.Utils;

/// <summary>
/// Helper extensions for FSM manipulation
/// </summary>
public static class FsmExtensions
{
    /// <summary>
    /// Inserts <paramref name="action"/> into <paramref name="state"/>'s actions at <paramref name="index"/>
    /// </summary>
    /// <param name="state">FSM state to modify</param>
    /// <param name="index">Index to insert at</param>
    /// <param name="action">Action to insert</param>
    public static void InsertAction(this FsmState state, int index, FsmStateAction action)
    {
        List<FsmStateAction> actions = state.Actions.ToList();
        actions.Insert(index, action);
        state.Actions = actions.ToArray();
    }

    /// <summary>
    /// Appends <paramref name="action"/> to <paramref name="state"/>'s actions
    /// </summary>
    /// <param name="state">FSM state to modify</param>
    /// <param name="action">Action to append</param>
    public static void AppendAction(this FsmState state, FsmStateAction action) => InsertAction(state, state.Actions.Length, action);
}
