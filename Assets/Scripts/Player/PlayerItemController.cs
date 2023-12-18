using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("Item Renderer")]
    [SerializeField] private GameObject itemRenderer;
    [SerializeField] private float itemDistance = 0.5f;

    [Header("Gun")]
    [SerializeField] private Gun gun;

    // Update is called once per frame
    void Awake(){
        //setting renderer size and shape
        itemRenderer.GetComponent<SpriteRenderer>().sprite = gun.sprite;
        itemRenderer.transform.localScale = gun.sprite_Size;
        gameObject.GetComponent<PlayerCameraController>().OnLayerChange += (object obj,  PlayerCameraController.LayerChangeArgs e) =>{
            itemRenderer.layer = e.layer;
        };
    }
    
    
    
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
                transform.position.y - mousePos.y,
                transform.position.x - mousePos.x
        );

        if(Input.GetKeyDown(KeyCode.E)) itemRenderer.layer = gameObject.layer;
        itemRenderer.transform.eulerAngles = new Vector3(0,0,angle * Mathf.Rad2Deg);
        itemRenderer.transform.position = new Vector3(
                    transform.position.x - Mathf.Cos(angle)* itemDistance,
                    transform.position.y - Mathf.Sin(angle)* itemDistance,
                    transform.position.z
        );
        

        if(Input.GetMouseButtonDown(0)){
            gun.Shoot(this, angle, transform.position, itemDistance, gameObject.layer);
        }
        
    }
}
