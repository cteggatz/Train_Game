using Pathfinding;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
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
    [SerializeField] private float damage, attckRange, attackForce, g_rayDistance;
    [SerializeField] private ParticleSystem death;
    [SerializeField] private AudioClip dsound, bite, jump;
    //[SerializeField] private int layermask;

    [SerializeField] Vector3 startOffset;

    private Path path;
    private int currentWaypoint = 0;
    [SerializeField] public bool isGrounded;
    Seeker seeker;
    Rigidbody2D rb;
    private bool isOnCoolDown;
    [SerializeField] private Transform oldtarget;

    public void Start()
    {
        gameObject.GetComponent<CircleCollider2D>().radius = awareness;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        isOnCoolDown = false;
        speed = 40 / health;
        oldtarget = target;
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        PathFollow();
        if(health <= 0)
        {
            AudioSource.PlayClipAtPoint(dsound, transform.position);
            Destroy(gameObject);
            Instantiate(death).transform.position = gameObject.transform.position;
        }
        if(gameObject.layer != target.gameObject.layer)
        {
            oldtarget = target; //broken?!
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Interactable");
            float mindist = Mathf.Infinity;
            for(int i = 0; i < doors.Length; i++){
                float dist = Vector3.Distance(doors[i].gameObject.transform.position, gameObject.transform.position);
                if(dist < mindist){
                    target = doors[i].transform;
                    mindist = dist;
                }
            }
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

        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed;

        // Jump
        if ( isGrounded && !isOnCoolDown)
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


    void OnCollisionEnter2D(Collision2D collision) {
            isGrounded = true;
            if (collision.gameObject.transform == target)
            {
                if (target.GetComponent<PlayerHealth>() != null)
                {
                    target.GetComponent<PlayerHealth>().health -= damage;
                    AudioSource.PlayClipAtPoint(bite, transform.position);
                }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<p_movement>() != null)
        {
            target = collision.gameObject.transform;
        }
        if (collision.IsTouching(gameObject.GetComponent<BoxCollider2D>()) && collision.gameObject.GetComponent<Furnace>() != null)
        {
            collision.gameObject.GetComponent<Furnace>().DamageTrain((int)damage);
        }
        if (collision.IsTouching(gameObject.GetComponent<BoxCollider2D>()) && collision.gameObject.GetComponent<DoorController>() != null)
        {
            if(gameObject.layer != oldtarget.gameObject.layer){
                gameObject.layer = oldtarget.gameObject.layer;
                target = oldtarget;// I think this is the bad boy
                //NEED TO FIX OLD TARGET BEING SET TO DOOR STILL SOMETIMES
            }
        }
    }
}