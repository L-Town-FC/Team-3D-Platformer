using UnityEngine;

public class feCirclingState : FlyingEnemyBaseState
{
    float circlingRandomizer = 1f;
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        //randomize the direction the enemy circles by just taking a
        //sin of the time
        circlingRandomizer = Mathf.Sign(Mathf.Sin(Time.time));

        enemy.ChangeSpeed(enemy.circlingSpeed);
        Debug.Log("Entering Circling");
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Leaving Circling");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        Vector3 enemyMovementInput = enemy.transform.right * circlingRandomizer;
        Vector3 newEnemyForward = (enemy.player.position - enemy.transform.position).normalized;

        enemyMovementInput += EnemyCirclingAdjustment(enemy);

        enemy.UpdateActorInputVectors(enemyMovementInput, newEnemyForward);
    }

    Vector3 EnemyCirclingAdjustment(FlyingEnemy enemy)
    {
        float enemyHorizontalPlayerDst = Vector3.ProjectOnPlane(enemy.player.position - enemy.transform.position, Vector3.up).magnitude;

        Vector3 horiztonalAdjustment = (enemyHorizontalPlayerDst - enemy.circlingRadius) * enemy.transform.forward;

        float enemyVerticalPlayerDst = enemy.transform.position.y - enemy.player.position.y;

        Vector3 verticalAdjustment = (enemyVerticalPlayerDst - enemy.defaultHeightAbovePlayer) * Vector3.down;

        return horiztonalAdjustment + verticalAdjustment;
        
    }
}
