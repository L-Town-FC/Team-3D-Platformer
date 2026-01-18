using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    float jumpForce = 6f;
    float maxJumpHoleTime = 0.2f;
    Vector3 jumpDir = Vector3.up;

    public override void EnterState(Player player)
    {
        stateEnterTime = Time.time;
        //if the player is on the wall then they should perform a wall jump instead of a normal jump
        if (player.OnWallCheck().Item1)
        {
            jumpDir += player.OnWallCheck().Item2.normalized;
        }
    }

    public override void ExitState(Player player)
    {
        jumpDir = Vector3.up;
    }

    public override void UpdateState(Player player)
    {
        //switches player out of jump state if they are either not trying to jump or have maxed out their jump
        if(!player.input.isJump || Time.time > stateEnterTime + maxJumpHoleTime)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        //grounded the player if they are grounded and not trying to jump
        //this check on isJump probably isnt required because if they are jumping they are going up
        //and it would be very rare for them to be grounded in that case
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
