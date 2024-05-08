using System.Collections;
using System.Collections.Generic;
using DataSaving;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Train_Interface_Controller : MonoBehaviour
{
    GameControllerInstance gameControllerInstance;

    [SerializeField] bool colliding;
    void Awake(){
        gameControllerInstance = FindObjectOfType<GameControllerInstance>();
    }

    void OnTriggerEnter2D(Collider2D coll){
        if(coll.gameObject.name == "Player" && gameControllerInstance.GetGameState() == GameData.GameState.Station){
            colliding = true;
        }
    }
    void OnTriggerExit2D(Collider2D coll){
        if(coll.gameObject.name == "Player"){
            colliding = false;
        }
    }

    void Update(){
        if(colliding && Input.GetKeyDown(KeyCode.E)){
            gameControllerInstance.SwitchScene(1);
        }
    }
}
