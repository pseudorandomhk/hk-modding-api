using HutongGames.PlayMaker;
using System;

namespace Modding.Utils;

/// <summary>
/// Wrapper for inserting arbitrary functions as FSM state actions
/// </summary>
public class FsmLambda : FsmStateAction
{
    private Action lambda;

    /// <summary>
    /// Create a new FsmLambda state action
    /// </summary>
    /// <param name="lambda">The function to run</param>
    public FsmLambda(Action lambda)
    {
        this.lambda = lambda;
    }

    /// <inheritdoc />
    public override void OnEnter()
    {
        base.OnEnter();
        lambda?.Invoke();
        base.Finish();
    }
}
