using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GruntAI : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private Transform target;
    [SerializeField] private float activateDistance, pathUpdateSeconds;

    [Header("Physics")]
    [SerializeField] private float speed;
    [SerializeField] private float nextWaypointDistance, jumpNodeHeightReq, jumpMod, jumpCheckOffset;

    [Header("Custom Behavior")]
    [SerializeField] private bool followEnabled;
    [SerializeField] private bool jumpEnabled, directionLookEnabled;

    private Path path;
    private int currentWaypoint = 0;
    private bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Vector2 currentVelocity;


    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }

    private void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone() && followEnabled)
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null || path.vectorPath.Count == 0)
        {
            return;
        }
        if (currentWaypoint >= path.vectorPath.Count) //If at target
        {
            return;
        }

        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized; // Find direction
        Vector2 force = direction * speed * Time.deltaTime;

        if (jumpEnabled && isGrounded && direction.y > jumpNodeHeightReq) //bouncy
        {
            rb.AddForce(Vector2.up * speed * jumpMod);
        }

        // Applying force
        if (!isGrounded) force.y = 0; //no workie, can still jump in the air
        rb.velocity = Vector2.SmoothDamp(rb.velocity, force, ref currentVelocity, 0.5f);

        // Waypoint reached check
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Look
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.position) < activateDistance;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}