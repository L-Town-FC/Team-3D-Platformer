using UnityEngine;

public class FlyingEnemy : ActorController
{
    float defaultHeightFromGround = 3f;
    float circlingRadius = 3f;
    bool isAlertedToPlayer = false;
    string playerTag = "Player";
    float idleSpeed = 3f;
    float swoopSpeed = 5f;
    Vector2 maxAndMinTimeBetweenSwoops = new Vector2(5f, 7f);
    public FlyingEnemyBaseState currentEnemyState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentEnemyState.UpdateState(this);

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public void ChangeState(FlyingEnemyBaseState _newState)
    {
        currentEnemyState.ExitState(this);
        currentEnemyState = _newState;
        currentEnemyState.EnterState(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isAlertedToPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isAlertedToPlayer = false;
        }
    }
}
