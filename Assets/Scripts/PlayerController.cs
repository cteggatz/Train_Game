using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovementScript : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float hMoveSpeed = 200f;
    [SerializeField] private float vMoveSpeed = 1000f;

    [Header("Camera settings")]
    [SerializeField] private CinemachineVirtualCamera vcam;


    private Rigidbody2D _rb;
    private BoxCollider2D _collider;

    private float hMoveDirection;
    private bool canJump;
    private bool canMoveOutside;
    private bool isOutside;


    void Start(){
        _rb = transform.GetComponent<Rigidbody2D>();
        _collider = transform.GetComponent<BoxCollider2D>();

        hMoveDirection = 0;
        canJump = true;
        canMoveOutside = false;
        isOutside = true;
    }

    // Update is called once per frame
    void Update(){
        hMoveDirection = Input.GetAxisRaw("Horizontal") * hMoveSpeed;

        if(Input.GetKey("e") == true && canMoveOutside){
            
            if(isOutside){
                vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = 2f;
                isOutside = false;
                this.gameObject.layer = 6;
            } else {
                vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance= 3f;
                isOutside = true;
                this.gameObject.layer = 7;
            }
            Debug.Log("Switching");
        }
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

    //adding layer changes
    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.tag == "Train_Exit"){
            canMoveOutside = true;
        }
    }
    //
    void OnTriggerExit2D(Collider2D other){
        if(other.transform.tag == "Train_Exit"){
            canMoveOutside = false;
        }
    }
}
