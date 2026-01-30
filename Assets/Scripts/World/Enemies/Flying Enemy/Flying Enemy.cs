using UnityEngine;

public class FlyingEnemy : ActorController
{
    float defaultHeightFromGround = 3f;
    float enemyAlertDst = 8f;
    public float circlingRadius = 3f;
    public bool isAlertedToPlayer = false;
    string playerTag = "Player";
    public float chaseSpeed = 1f;
    float swoopSpeed = 5f;
    Vector2 maxAndMinTimeBetweenSwoops = new Vector2(5f, 7f);
    public FlyingEnemyBaseState currentEnemyState;
    [HideInInspector]
    public Transform player;
    [SerializeField]
    SphereCollider chaseCollider;

    public feIdleState idleState = new feIdleState();
    public feChaseState chaseState = new feChaseState();
    public feCirclingState circlingState = new feCirclingState();
    public feSwoopState swoopState = new feSwoopState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        chaseCollider.radius = enemyAlertDst;

        currentEnemyState = idleState;
        currentEnemyState.EnterState(this);
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

    //method called by enemy state classes to update this base classes movement and direction
    public void UpdateActorInputVectors(Vector3 _inputVector, Vector3 _newForward)
    {
        base.inputVector = _inputVector;
        base.newTransformForward = _newForward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isAlertedToPlayer = true;
            player = other.transform;
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
