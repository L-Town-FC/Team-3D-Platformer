using UnityEngine;

public class PlayerSuperJumpState : PlayerBaseState
{
    //player completes jump that is higher and longer than normal jump and
    //cant be cancelled once started

    float superJumpTimeLength = 0.3f;
    float superJumpForce = 20f;
    float gracePeriod = 0.1f;
    Vector3 jumpDir;
    public override void EnterState(PlayerV2 player)
    {
        Debug.Log("Entering Super Jump");
        stateEnterTime = Time.time;
        //0.2f is arbitrary. just sends the player backwards slightly like they are doing a backflip
        jumpDir = Vector3.up - (player.transform.forward.normalized * 0.2f);
    }

    public override void ExitState(PlayerV2 player)
    {
        Debug.Log("Exiting Super Jump");
    }

    public override void UpdateState(PlayerV2 player)
    {
        Vector3 newMovement = ApplyJump(jumpDir, superJumpForce, stateEnterTime, superJumpTimeLength);
        Vector3 newForward = player.transform.forward;

        player.UpdateActorInputVectors(newMovement, newForward);

        //include a small grace period so the player can actually start to get off the ground before doing a ground check
        if (player.isPlayerGrounded && Time.time > stateEnterTime + gracePeriod)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if(Time.time > stateEnterTime + superJumpTimeLength)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

    }

    Vector3 ApplyJump(Vector3 _jumpDir, float _jumpForce, float _jumpStartTime, float _maxJumpHoldTime)
    {
        // holding jump increases height of jump, diminishing until maxJumpHoldTime
        return _jumpDir * _jumpForce *
                (1f - Mathf.InverseLerp(_jumpStartTime, _jumpStartTime + _maxJumpHoldTime, Time.time));
    }
}
