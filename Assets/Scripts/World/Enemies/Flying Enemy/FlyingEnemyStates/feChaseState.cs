using UnityEngine;

public class feChaseState : FlyingEnemyBaseState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        Debug.Log("Entering Chase");
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Leaving Chase");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        Vector3 dirToPlayer = enemy.player.position - enemy.transform.position;

        enemy.UpdateActorInputVectors(dirToPlayer.normalized * enemy.chaseSpeed, dirToPlayer.normalized);

        if (EnemyInCirclingRange(enemy))
        {
            enemy.ChangeState(enemy.circlingState);
        }
    }

    bool EnemyInCirclingRange(FlyingEnemy enemy)
    {
        Vector2 enemyPositionNoVert = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
        Vector2 playerPositionNoVert = new Vector2(enemy.player.position.x, enemy.player.position.z);
        return Vector2.Distance(enemyPositionNoVert, playerPositionNoVert) <= enemy.circlingRadius;
    }
}
