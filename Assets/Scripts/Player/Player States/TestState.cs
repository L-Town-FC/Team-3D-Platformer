using UnityEngine;

public class TestState : PlayerBaseState
{
    public override void EnterState(PlayerV2 player)
    {
        Debug.Log("Entering Test State");
    }

    public override void ExitState(PlayerV2 player)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerV2 player)
    {
        Debug.Log("Ive switched States!");
    }
}
