using UnityEngine;

public class PlayerV2 : ActorController
{
    public PlayerBaseState currentPlayerState;
    public bool isPlayerGrounded => isGrounded;

    public PlayerInput input;
    public Collision playerCollision => actorCollision;

    public PlayerGroundState pGroundState;
    public PlayerIdleAirState pIdleAirState;
    public PlayerJumpState pJumpState;
    public PlayerWallState pWallState;
    public PlayerCrouchState pCrouchState;
    public PlayerSuperJumpState pSuperJumpState;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        pGroundState = new PlayerGroundState();
        pIdleAirState = new PlayerIdleAirState();
        pJumpState = new PlayerJumpState();
        pWallState = new PlayerWallState();
        pCrouchState = new PlayerCrouchState();
        pSuperJumpState = new PlayerSuperJumpState();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.inputVector = Vector3.zero;
        base.newTransformForward = transform.forward;

        currentPlayerState = pGroundState;
     
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentPlayerState.UpdateState(this);

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void ChangeState(PlayerBaseState _newState)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = _newState;
        currentPlayerState.EnterState(this);
    }

    public void UpdateActorInputVectors(Vector3 _inputVector, Vector3 _newForward)
    {
        base.inputVector = _inputVector;
        base.newTransformForward = _newForward;
    }

    public Vector3 CamRelativeInputVector()
    {
        Vector3 camRelative = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized;
        return camRelative;
    }

    public (bool, Vector3) OnWallCheck()
    {
        //OnWallCheck
        //Walls must be perfectly vertical to count as walls
        //if they are vertical then the normal vectors must have no y-component
        int wallContacts = 0;
        Vector3 wallJumpDir = Vector3.up;

        foreach (ContactPoint contact in actorCollision.contacts)
        {
            if (contact.normal.y == 0)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.green);
                //the direction exactly opposite the wall
                wallJumpDir = -contact.normal.normalized;
                wallContacts++;
            }
        }

        //if atleast two points on the player are touching a wall, the player is considered on the wall
        return (wallContacts > 1, wallJumpDir);
    }
}
