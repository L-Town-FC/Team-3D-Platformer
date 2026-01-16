using UnityEngine;

public class PlayerBounceState : PlayerBaseState
{
    float bounceForce = 5f;
    float bounceTimeLength = 0.2f;

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

        newMovement += ApplyJump(Vector3.up, bounceForce, stateEnterTime, bounceTimeLength);

        player.UpdateActorInputVectors(newMovement, newForward);

        if(Time.time > stateEnterTime + bounceTimeLength)
        {
            player.ChangeState(player.pIdleAirState);
        }
    }

    Vector3 ApplyJump(Vector3 _jumpDir, float _jumpForce, float _jumpStartTime, float _maxJumpHoldTime)
    {
        // holding jump increases height of jump, diminishing until maxJumpHoldTime
        return _jumpDir * _jumpForce *
                (1f - Mathf.InverseLerp(_jumpStartTime, _jumpStartTime + _maxJumpHoldTime, Time.time));
    }
}
