using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Train_Interface_Controller : MonoBehaviour
{
    GameControllerInstance gameControllerInstance;
    void Awake(){
        gameControllerInstance = FindObjectOfType<GameControllerInstance>();
    }

    void OnTriggerEnter2d(Collider2D coll){
        if(coll.gameObject.name == "player"){
            Debug.Log("Colliding!");
        }
    }
}
