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
            Debug.DrawLine(
                transform.position,
                Camera.main.ScreenToWorldPoint(new Vector3(
                    Input.mousePosition.x,
                    Input.mousePosition.y,
                    0
                )),
                Color.red,
                2f
            );
        }        




    }
}
