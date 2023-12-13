using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShootingScript : MonoBehaviour
{
    
    private Rigidbody2D _rb;


    void Start()
    {
        _rb = transform.GetComponent<Rigidbody2D>();
    }

    void Update(){
        if(Input.GetButtonDown("Fire1")){
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float xPos = transform.position.x - mousePos.x;
            float yPos = transform.position.y - mousePos.y;

            float angle = -Mathf.Atan2(yPos, xPos) * Mathf.Rad2Deg;
            Debug.Log($"x {xPos} | y {yPos} | angle {angle}");
        }        




    }
}
