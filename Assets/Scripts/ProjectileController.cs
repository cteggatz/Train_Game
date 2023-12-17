using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    
    [SerializeField] private float thrust = 10;

    private Rigidbody2D _rb;

    private void Awake(){
        _rb = gameObject.GetComponent<Rigidbody2D>();

        
        _rb.gravityScale = 0;
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        _rb.velocity = new Vector2(
            -Mathf.Cos(angle) * thrust,
            -Mathf.Sin(angle) * thrust
        );
    }
    private void Update(){
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(_rb.position);
        if(screenPosition.x > Screen.width || screenPosition.y > Screen.height || screenPosition.x < 0 || screenPosition.y < 0) {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        Destroy(gameObject);
    }
    public void SetLayers(int layer){

        gameObject.layer = layer;

        Collider2D _col = gameObject.GetComponent<Collider2D>();
        _col.includeLayers ^= 1 << gameObject.layer;   

        _col.excludeLayers ^= 1 << ((gameObject.layer == 7) ? 6 : 7);
    }
}
