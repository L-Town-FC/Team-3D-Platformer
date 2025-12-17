using UnityEngine;

public class BasicEnemy : PlayerController
{
    [SerializeField] private Transform player;
    [SerializeField] private float stopDistance = 0.75f;

    Vector3 dir = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (player == null)
            return;
        dir =  player.position - transform.position;
        base.newTransformForward = dir;

        float dist = dir.magnitude;
        if (dist <= stopDistance || dist < 0.001f)
        {
            return;
        }

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.inputVector = Vector3.forward;
        base.FixedUpdate();
    }
}
