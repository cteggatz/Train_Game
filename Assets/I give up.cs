using Pathfinding;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Igiveup : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private Transform target;
    [SerializeField] private float activateDistance, pathUpdateSeconds;

    [Header("Physics")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce, nextWaypointDistance, jumpNodeHeightRequirement, jumpModifier, jumpCheckOffset;

    [Header("Custom Behavior")]
    [SerializeField] private bool isInAir;
    [SerializeField] private float attckRange, attackForce, damage, g_rayDistance, health;
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
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        isInAir = false;
        isOnCoolDown = false; 

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance())
        {
            PathFollow();
        }
        if(Vector2.Distance(transform.position, target.transform.position) < attckRange){
            Attack();
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdatePath()
    {
        if (TargetInDistance() && seeker.IsDone())
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
        isGrounded = Physics2D.Raycast(gameObject.transform.position, -Vector2.up, g_rayDistance);
        //Debug.DrawRay(gameObject.transform.position, -Vector2.up, Color.green, g_rayDistance);

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

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
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
            Debug.Log("HIT");
            target.GetComponent<PlayerHealth>().health -= damage;
            //if (target.GetComponent<PlayerHealth>() != null){ //FUCK
         //       target.GetComponent<PlayerHealth>().health -= damage;
         //       StartCoroutine(AttackCoolDown());
       //     }
        }
        if(collision.gameObject.GetComponent<ProjectileScript>() != null) //BROKE
        {
            Debug.Log("OW");
            health -= 1f;
        }
    }
    
}