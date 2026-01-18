using UnityEngine;

public class PlayerBounceState : PlayerBaseState
{
    float bounceForce = 6f;
    float bounceTimeLength = 0.2f;

    public override void EnterState(Player player)
    {
        stateEnterTime = Time.time;
    }

    public override void ExitState(Player player)
    {

    }

    public override void UpdateState(Player player)
    {
        Vector3 newMovement = player.CamRelativeInputVector();
        Vector3 newForward = newMovement;

        //bounce is applied the same as a jump just with different initial force and its uncancelable
        newMovement += ApplyJump(Vector3.up, bounceForce, stateEnterTime, bounceTimeLength);

        player.UpdateActorInputVectors(newMovement, newForward);

        //the player is bouncing so they must be in the air when the bounce velocity stops being applied
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
