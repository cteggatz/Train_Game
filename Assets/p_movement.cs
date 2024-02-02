using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class p_movement : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 velocity;
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothTime, walkspeed, maxSpeed, jumphight, coyoteTimeInterval, jumpBufferInterval;
    private float timeLastOnGround, timeLastPressedJump;
    private bool grounded;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timeLastOnGround += Time.deltaTime;
        timeLastPressedJump += Time.deltaTime;
        if(Time.time - timeLastOnGround <= coyoteTimeInterval){
            grounded = true;
        }
        float targetHorizontalSpeed = Input.GetAxisRaw("Horizontal") * walkspeed;
        body.velocity = Vector2.SmoothDamp(body.velocity, new Vector2(targetHorizontalSpeed, body.velocity.y), ref velocity, smoothTime, maxSpeed);
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));//animation stuff
        if (gameObject.transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x) //looks in the direction of the mouse
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        //if(Input.GetAxisRaw("Vertical") > 0){
        //    timeLastPressedJump = 0;
        //}
        if (Input.GetAxisRaw("Vertical") > 0 && grounded){
            body.velocity = new Vector2(body.velocity.x, jumphight);
            grounded = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        timeLastOnGround = 0;
        grounded = true;
    }
}
