using UnityEngine;

public class PlayerWallState : PlayerBaseState
{
    public override void EnterState(PlayerV2 player)
    {
        Debug.Log("Entering Wall State");
        throw new System.NotImplementedException();
    }

    public override void ExitState(PlayerV2 player)
    {
        Debug.Log("Exiting Wall State");
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerV2 player)
    {
        //Make player always face away from wall

        //Stop lateral movement and movement towards wall

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if (player.input.isJump)
        {
            player.ChangeState(player.pJumpState);
            return;
        }

        if (!player.OnWallCheck().Item1)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        throw new System.NotImplementedException();
    }
}
