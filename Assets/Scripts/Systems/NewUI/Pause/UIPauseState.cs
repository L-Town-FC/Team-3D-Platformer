using UnityEngine;

public class UIPauseState : UIBaseState
{
    //TODO: Create Pause menu object
    //TODO: Modify Pause menu
    //TODO: Create Death menu
    protected override BaseMenu UIMenu { get; set; }

    public override void EnterState(UIStateManager ui)
    {
        stateEnterTime = Time.time;
        //throw new System.NotImplementedException();
    }

    public override void ExitState(UIStateManager ui)
    {
        //throw new System.NotImplementedException();
    }

    public override void UpdateState(UIStateManager ui)
    {
        //throw new System.NotImplementedException();
    }
}
