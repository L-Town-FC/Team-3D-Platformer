using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    //TODO: Add check for if "OnWall" to see if this jump should be a normal jump or wall jump

    float jumpForce = 6f;
    float maxJumpHoleTime = 0.2f;
    Vector3 jumpDir = Vector3.up;

    public override void EnterState(PlayerV2 player)
    {
        stateEnterTime = Time.time;
        if (player.OnWallCheck().Item1)
        {
            jumpDir += player.OnWallCheck().Item2.normalized;
        }
        Debug.Log("Entering Jump State");
    }

    public override void ExitState(PlayerV2 player)
    {
        jumpDir = Vector3.up;
        Debug.Log("Exiting Jump State");
    }

    public override void UpdateState(PlayerV2 player)
    {
        if(!player.input.isJump || Time.time > stateEnterTime + maxJumpHoleTime)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        if (player.isPlayerGrounded && !player.input.isJump)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        Vector3 newInput = player.CamRelativeInputVector();
        Vector3 newForward = newInput;

        if (player.input.isJump)
        {
            newInput += ApplyJump(jumpDir, jumpForce, stateEnterTime, maxJumpHoleTime);
        }

        player.UpdateActorInputVectors(newInput, newForward);
    }

    Vector3 ApplyJump(Vector3 _jumpDir, float _jumpForce, float _jumpStartTime, float _maxJumpHoldTime)
    {
        // holding jump increases height of jump, diminishing until maxJumpHoldTime
        return _jumpDir * _jumpForce *
                (1f - Mathf.InverseLerp(_jumpStartTime, _jumpStartTime + _maxJumpHoldTime, Time.time));
    }
}
