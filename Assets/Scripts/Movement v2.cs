using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Movementv2 : MonoBehaviour
{
    [SerializeField] private float DefaultSpeed, SprintSpeed, CurrentSpeed, SpeedLoss, SprintGain;
    [SerializeField] private float jumphight;
    [SerializeField] private float climbMultiplier;
    private Rigidbody2D body;
    private bool grounded;
    private bool canClimb = false ;
    private bool isClimbing = false;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        CurrentSpeed += DefaultSpeed;
    }

    private void Update()
    {
        /*
            @Tino pls add comments so i can understand wtf is happening in here. - Chris
        */

        if (Input.GetKey(KeyCode.LeftShift) && grounded && CurrentSpeed < SprintSpeed)
        {
            CurrentSpeed += SprintGain;
        }
        else if (CurrentSpeed > DefaultSpeed)
        {
            CurrentSpeed -= SpeedLoss;
        }
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * CurrentSpeed, body.velocity.y); //Actual move part
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0) // Flippy flippy
        {
            transform.localRotation = Quaternion.Euler(0, horizontalInput > 0 ? 0 : 180, 0);
        }


        if(Input.GetKey(KeyCode.W) && grounded){
            body.velocity = new Vector2(body.velocity.x, jumphight);
            grounded = false;
        }
        
        if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0 && canClimb){
            isClimbing = true;
        } else {
            isClimbing = false;
        }
    }
    private void FixedUpdate(){
        float verticalAxis = Input.GetAxisRaw("Vertical");
        if(isClimbing){
            body.velocity = new Vector2(body.velocity.x, jumphight * climbMultiplier * verticalAxis);
        } else if(verticalAxis == 0 && canClimb){
            body.velocity = new Vector2(body.velocity.x, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
            grounded = true;
    }

    public void SetCanClimb(bool state){
        canClimb = state;
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
