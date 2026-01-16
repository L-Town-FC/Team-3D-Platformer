using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    
    public override void EnterState(PlayerV2 player)
    {
        stateEnterTime = Time.time;
    }

    public override void ExitState(PlayerV2 player)
    {
    }

    public override void UpdateState(PlayerV2 player)
    {
        Vector3 newMovement = player.CamRelativeInputVector();
        Vector3 newForward = newMovement;

        player.UpdateActorInputVectors(newMovement, newForward);

        if (!player.isPlayerGrounded)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        if (player.input.isJump)
        {
            player.ChangeState(player.pJumpState);
            return;
        }

        if (player.input.isCrouching)
        {
            player.ChangeState(player.pCrouchState);
            return;
        }
    }
}
