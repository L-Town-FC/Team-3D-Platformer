using UnityEngine;

public class FlyingEnemy : ActorController
{
    public float defaultHeightAbovePlayer = 3f;
    float enemyAlertDst = 10f;
    public float circlingRadius = 5f;
    public bool isAlertedToPlayer = false;
    string playerTag = "Player";
    public float chaseSpeed = 1f;
    public float circlingSpeed = 3f;
    public float swoopSpeed = 5f;
    public Vector2 minAndMaxTimeBetweenSwoops = new Vector2(5f, 7f);
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
        base.airDrag = base.groundDrag;
        ChangeSpeed(chaseSpeed, chaseSpeed);

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

    public void ChangeSpeed(float _horizontalSpeed, float _verticalSpeed)
    {
        speed = _horizontalSpeed;
        minAndMaxVerticalMovementSpeed = new Vector2(-_verticalSpeed, _verticalSpeed);
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
