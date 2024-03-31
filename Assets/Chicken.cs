using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    [SerializeField] private float pathUpdateSeconds;

    [Header("Physics")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce, nextWaypointDistance, jumpNodeHeightRequirement, jumpModifier, jumpCheckOffset;

    [Header("Custom Behavior")]
    [SerializeField] private ParticleSystem death;
    [SerializeField] private AudioClip dsound, bite, jump;

    [SerializeField] Vector3 startOffset;

    private Path path;
    private int currentWaypoint = 0;
    [SerializeField] private bool isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    private bool isOnCoolDown;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        isOnCoolDown = false;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        PathFollow();
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed;

        // Jump
        if (isGrounded && !isOnCoolDown)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                AudioSource.PlayClipAtPoint(jump, transform.position);
                if (!isGrounded) return;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                StartCoroutine(JumpCoolDown());
            }
        }

        // Movement
        rb.velocity = new Vector2(force.x, rb.velocity.y);

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (rb.velocity.x > 0.05f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x < -0.05f)
        {
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }


    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    IEnumerator JumpCoolDown()
    {
        isOnCoolDown = true;
        yield return new WaitForSeconds(1f);
        isOnCoolDown = false;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<ProjectileScript>() != null)
        {
            AudioSource.PlayClipAtPoint(dsound, transform.position);
            Destroy(gameObject);
            Instantiate(death).transform.position = gameObject.transform.position;
        }
    }
}
