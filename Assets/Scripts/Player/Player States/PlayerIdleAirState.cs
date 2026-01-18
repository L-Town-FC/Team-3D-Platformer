using UnityEngine;

public class PlayerIdleAirState : PlayerBaseState
{
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

        player.UpdateActorInputVectors(newMovement, newForward);

        //checks if player is on top of bounceable entity
        if (BounceCheck(player))
        {
            player.ChangeState(player.pBounceState);
            return;
        }

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if (player.OnWallCheck().Item1)
        {
            player.ChangeState(player.pWallState);
            return;
        }
    }

    bool BounceCheck(Player _player)
    {
        //currently only bouncing on entities with the "Enemy" layer applied
        LayerMask bounceableEntityMask = LayerMask.GetMask("Enemy");

        //player should only be able to bounce if they are moving downward
        if(_player.AppliedMovmement.y > 0f)
        {
            return false;
        }

        //check a sphere at the players feet
        //if it hits a collider that has been marked as a bounceable entity
        //its added to the array of colliders
        Collider[] colliders = Physics.OverlapSphere(_player.transform.position - Vector3.up, 0.65f, bounceableEntityMask, QueryTriggerInteraction.Ignore);

        //if nothing is detected the player should not bounce
        if(colliders.Length < 1)
        {
            return false;
        }

        foreach(Collider col in colliders)
        {
            //apply damage to object the player is bouncing on if possible
            if(col.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(_player.stompDamage);
            }
        }

        return true;
    }
}
