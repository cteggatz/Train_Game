using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Movementv2 : MonoBehaviour
{
    [SerializeField] private float jumphight, speed;
    [SerializeField] private float climbMultiplier;
    [SerializeField] private Animator animator;
    private Rigidbody2D body;
    private bool grounded;
    /* private bool canClimb = false ;
    private bool isClimbing = false; */


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update(){
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y); //Actual move part
        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));//animation stuff
        if (gameObject.transform.position.x > Camera.main.ScreenToWorldPoint(Input.mousePosition).x) //looks in the direction of the mouse
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else{
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }


        if(Input.GetAxisRaw("Vertical") > 0 && grounded){
            body.velocity = new Vector2(body.velocity.x, jumphight);
            grounded = false;
        }
        /*
        if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0 && canClimb){
            isClimbing = true;
        } else {
            isClimbing = false;
        } */
    }
    /* private void FixedUpdate(){
        float verticalAxis = Input.GetAxisRaw("Vertical");
        if(isClimbing){
            body.velocity = new Vector2(body.velocity.x, jumphight * climbMultiplier * verticalAxis);
        } else if(verticalAxis == 0 && canClimb){
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    } */

    private void OnCollisionEnter2D(Collision2D collision)
    {
            grounded = true;
    }

     public void SetCanClimb(bool state){
       // canClimb = state;
        if(grounded){
            grounded = !grounded;
        }
        if(state){
            body.gravityScale = 0;
        }else {
            body.gravityScale = 1f;
        }
    } 
}
