using UnityEngine;

public class feCirclingState : FlyingEnemyBaseState
{
    float circlingRandomizer = 1f; //used to randomize circling direction
    float bufferDst = 1f; //small buffer to let enemy swoop when not exactly on the circling radius
    float swoopTime; //Time until next swoop
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        //randomize the direction the enemy circles by just taking a
        //sin of the time
        circlingRandomizer = Mathf.Sign(Mathf.Sin(Time.time));
        swoopTime = Random.Range(enemy.minAndMaxTimeBetweenSwoops.x, enemy.minAndMaxTimeBetweenSwoops.y);

        enemy.ChangeSpeed(enemy.circlingSpeed, enemy.circlingSpeed);
    }

    public override void ExitState(FlyingEnemy enemy)
    {
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        Vector3 enemyMovementInput = enemy.transform.right * circlingRandomizer;
        Vector3 newEnemyForward = (enemy._player.transform.position - enemy.transform.position).normalized;

        enemyMovementInput += EnemyCirclingAdjustment(enemy);

        enemy.UpdateActorInputVectors(enemyMovementInput, newEnemyForward);

        if (SwoopCheck(enemy))
        {
            enemy.ChangeState(enemy.swoopState);
        }
    }

    Vector3 EnemyCirclingAdjustment(FlyingEnemy enemy)
    {
        //tries to keep enemy at the circling radius and at the predefined height above the player
        float enemyHorizontalPlayerDst = EnemyToPlayerHorizontalDst(enemy);

        Vector3 horiztonalAdjustment = (enemyHorizontalPlayerDst - enemy.circlingRadius) * enemy.transform.forward;

        float enemyVerticalPlayerDst = enemy.transform.position.y - enemy._player.position.y;

        Vector3 verticalAdjustment = (enemyVerticalPlayerDst - enemy.defaultHeightAbovePlayer) * Vector3.down;

        return horiztonalAdjustment + verticalAdjustment;
        
    }

    float EnemyToPlayerHorizontalDst(FlyingEnemy enemy)
    {
        return Vector3.ProjectOnPlane(enemy._player.position - enemy.transform.position, Vector3.up).magnitude;
    }

    bool SwoopCheck(FlyingEnemy enemy)
    {
        //Checks if enemy should swoop on the player
        //player and enemy need to be within a small distance band
        //time check to stop enemy from swooping constanly
        float dst = EnemyToPlayerHorizontalDst(enemy);
        float radius = enemy.circlingRadius;
        if(dst > radius + bufferDst || dst < radius - bufferDst)
        {
            return false;
        }

        if(Time.time < stateEnterTime + swoopTime)
        {
            return false;
        }

        return true;
    }
}
