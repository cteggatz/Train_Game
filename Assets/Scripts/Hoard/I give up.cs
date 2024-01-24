using Pathfinding;
using System.Collections;
using UnityEngine;

public class Igiveup : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float awareness, health;
    [SerializeField] private float pathUpdateSeconds;

    [Header("Physics")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce, nextWaypointDistance, jumpNodeHeightRequirement, jumpModifier, jumpCheckOffset;

    [Header("Custom Behavior")]
    [SerializeField] private bool isInAir;
    [SerializeField] private float attckRange, attackForce, damage, g_rayDistance;
    [SerializeField] private ParticleSystem death;
    [SerializeField] private AudioClip dsound, bite;
    //[SerializeField] private int layermask;

    [SerializeField] Vector3 startOffset;

    private Path path;
    private int currentWaypoint = 0;
    private RaycastHit2D isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    private bool isOnCoolDown;

    public void Start()
    {
        gameObject.GetComponent<CircleCollider2D>().radius = awareness;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        isInAir = false;
        isOnCoolDown = false; 

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        PathFollow();
        if(Vector2.Distance(transform.position, target.transform.position) < attckRange){
            //Attack();
        }
        if(health <= 0)
        {
            AudioSource.PlayClipAtPoint(dsound, transform.position);
            Destroy(gameObject);
            Instantiate(death).transform.position = gameObject.transform.position;
        }
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

        // See if colliding with anything
        isGrounded = Physics2D.Raycast(gameObject.transform.position, -Vector3.up, g_rayDistance);
        //Debug.DrawRay(startOffset, -Vector3.up, Color.blue, g_rayDistance);

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed;

        // Jump
        if ( isGrounded && !isInAir && !isOnCoolDown)
        {
            if (direction.y > jumpNodeHeightRequirement)
            {
                if (isInAir) return; 
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                StartCoroutine(JumpCoolDown());
            }
        }
        if (isGrounded)
        {
            isInAir = false; 
        }
        else
        {
            isInAir = true;
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
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
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

    IEnumerator AttackCoolDown()
    {
        isOnCoolDown = true; 
        yield return new WaitForSeconds(2f);
        isOnCoolDown = false;
    }

    private void Attack(){
        if (isInAir) return; 
        rb.velocity = new Vector2(rb.velocity.x, attackForce);
        StartCoroutine(JumpCoolDown());
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.transform == target){
            if (target.GetComponent<PlayerHealth>() != null){
                target.GetComponent<PlayerHealth>().health -= damage;
                AudioSource.PlayClipAtPoint(bite, transform.position);
                // StartCoroutine(AttackCoolDown());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Movementv2>() != null)
        {
            target = collision.gameObject.transform;
        }
    }
}