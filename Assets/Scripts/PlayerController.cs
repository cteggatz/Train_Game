using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovementScript : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float hMoveSpeed = 200f;
    [SerializeField] private float vMoveSpeed = 1000f;

    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    private float hMoveDirection;
    private bool canJump;
    


    void Start(){
        _rb = transform.GetComponent<Rigidbody2D>();
        _collider = transform.GetComponent<BoxCollider2D>();

        hMoveDirection = 0;
        canJump = true;

    }

    // Update is called once per frame
    void Update(){
        hMoveDirection = Input.GetAxisRaw("Horizontal") * hMoveSpeed;
    }

    void FixedUpdate(){
        if(canJump){
            _rb.AddForce(transform.right * hMoveDirection);

            if(Input.GetAxisRaw("Vertical") > 0){
                canJump = false;
                _rb.AddForce(transform.up * vMoveSpeed);
            } 
        } else {
            _rb.AddForce(transform.right * hMoveDirection/10);
        }


    }
    void OnCollisionEnter2D(Collision2D other){
        canJump = true;
    }

    
}
