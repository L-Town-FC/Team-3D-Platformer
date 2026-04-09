using UnityEngine;

public class UIPauseState : UIBaseState
{
    protected override BaseMenu UIMenu { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        throw new System.NotImplementedException();
    }

    public override void ExitState(UIStateManager ui)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(UIStateManager ui)
    {
        throw new System.NotImplementedException();
    }
}
