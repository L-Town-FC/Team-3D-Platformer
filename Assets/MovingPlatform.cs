using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    GameObject Platform; //actual platform that will move
    Rigidbody rb; //rigidbody attached to actual platform
    [SerializeField]
    List<Transform> PlatformWaypoints = new List<Transform>(); //list of transforms for all waypoints the platform will travel to
    //This is a list of transforms instead of just coords so you can move the points around in the editor

    [SerializeField]
    int nextWaypoint = 1; //holds the next waypoint index
    int snakeDir = 1; //when snakeMode is enabled this holds whether the platform is incrementing or decrementing waypoints
    Vector3 velocity = Vector3.zero; //reference velocity to be used by Smooth Damp
    [SerializeField]
    float timeBetweenWaypoints = 1f; //how much time it takes to get from one waypoint to another
    //currently a static value so different distances will have different speeds
    //this possibly should be modified to by dynamic in order to keep a constant speed between waypoints

    [SerializeField]
    bool snakeMovement = false; //determines if platform should loop or snake
    //loop 0->1->2->0, etc
    //snake 0->1->2->1->0, etc

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Platform = transform.GetChild(0).gameObject;
        rb = Platform.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(Vector3.SmoothDamp(rb.position, PlatformWaypoints[nextWaypoint].position, ref velocity, timeBetweenWaypoints));
        //0.01f value is arbitrary
        //just needed a small value in order to check that the platform has reached the waypoint position and avoid floating point precision comparison issues
        if ((rb.position - PlatformWaypoints[nextWaypoint].position).sqrMagnitude < 0.01f)
        {
            SetWaypoints();
        }
    }

    void SetWaypoints()
    {
        if (!snakeMovement)
        {
            //if not snake, simply reset the next waypoint to 0 if you are at the end of the list
            if(nextWaypoint + 1 >= PlatformWaypoints.Count)
            {
                nextWaypoint = 0;
                return;
            }
            nextWaypoint += 1;
            return;
        }

        //if in snake, check if you were incrementing or decrementing, then switch direction if an index out of range error was about to occur
        if(nextWaypoint + snakeDir >= PlatformWaypoints.Count || nextWaypoint + snakeDir < 0)
        {
            snakeDir *= -1;
        }

        nextWaypoint += snakeDir;
        return;
    }
}
