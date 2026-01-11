using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    float jumpForce = 6f;
    float maxJumpHoleTime = 0.2f;

    public override void EnterState(PlayerV2 player)
    {
        stateEnterTime = Time.time;
        Debug.Log("Entering Jump State");
    }

    public override void ExitState(PlayerV2 player)
    {
        Debug.Log("Exiting Jump State");
    }

    public override void UpdateState(PlayerV2 player)
    {
        if(!player.input.isJump || Time.time > stateEnterTime + maxJumpHoleTime)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        Vector3 newInput = CamRelativeInputVector(player.input);
        Vector3 newForward = newInput;

        if (player.input.isJump)
        {
            newInput += ApplyJump(Vector3.up, jumpForce, stateEnterTime, maxJumpHoleTime);
        }

        player.UpdateActorInputVectors(newInput, newForward);
    }

    Vector3 ApplyJump(Vector3 _jumpDir, float _jumpForce, float _jumpStartTime, float _maxJumpHoldTime)
    {
        // holding jump increases height of jump, diminishing until maxJumpHoldTime
        return _jumpDir * _jumpForce *
                (1f - Mathf.InverseLerp(_jumpStartTime, _jumpStartTime + _maxJumpHoldTime, Time.time));
    }

    //Converts input vector from world vector to input vector based on the camera
    //makes movement feel better
    Vector3 CamRelativeInputVector(PlayerInput playerInput)
    {
        //    Your old line was:
        //      base.inputVector = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        //
        //    That can introduce a NEGATIVE Y when the camera is tilted down,
        //    which fights your stomp bounce. So we project onto the XZ plane.
        Vector3 camRelative = Camera.main.transform.TransformDirection(playerInput.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized; // <-- NEW
        return camRelative;
    }
}
