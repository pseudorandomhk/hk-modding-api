namespace HutongGames.PlayMaker.Actions;

#pragma warning disable CS1591 // suppress missing doc warning

/// <summary>
/// Port of <c>HutongGames.PlayMaker.Actions.TakeDamage</c> FSM action
/// </summary>
public class TakeDamage : FsmStateAction
{
    public override void Reset()
    {
        base.Reset();
        this.Target = new FsmGameObject
        {
            UseVariable = true
        };
        this.AttackType = new FsmInt
        {
            UseVariable = true
        };
        this.CircleDirection = new FsmBool
        {
            UseVariable = true
        };
        this.DamageDealt = new FsmInt
        {
            UseVariable = true
        };
        this.Direction = new FsmFloat
        {
            UseVariable = true
        };
        this.IgnoreInvulnerable = new FsmBool
        {
            UseVariable = true
        };
        this.MagnitudeMultiplier = new FsmFloat
        {
            UseVariable = true
        };
        this.MoveAngle = new FsmFloat
        {
            UseVariable = true
        };
        this.MoveDirection = new FsmBool
        {
            UseVariable = true
        };
        this.Multiplier = new FsmFloat
        {
            UseVariable = true
        };
        this.SpecialType = new FsmInt
        {
            UseVariable = true
        };
    }

    public override void OnEnter()
    {
        base.OnEnter();
        base.Finish();
    }

    public TakeDamage() { }

    public FsmGameObject Target;

    public FsmInt AttackType;

    public FsmBool CircleDirection;

    public FsmInt DamageDealt;

    public FsmFloat Direction;

    public FsmBool IgnoreInvulnerable;

    public FsmFloat MagnitudeMultiplier;

    public FsmFloat MoveAngle;

    public FsmBool MoveDirection;

    public FsmFloat Multiplier;

    public FsmInt SpecialType;
}
