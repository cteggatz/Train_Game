using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState{
    Exit_Left,
    ExitRight,
    Optional,
    Exit_Up
}


public class DoorController : MonoBehaviour
{

    [Header("Door Settings")]
    [SerializeField] private DoorState state;

    


    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name != "Player"){
            return;
        }
        if(state == DoorState.Optional){
            other.gameObject.GetComponent<PlayerCameraController>().SetMoveOutside(true);
            //Debug.Log("Can move outside");
        }  
    }

    void OnTriggerStay2D(Collider2D other){
        if(other.gameObject.name == "Player" && state == DoorState.Optional && other.gameObject.GetComponent<PlayerCameraController>().GetMoveOutside() == false){
            other.gameObject.GetComponent<PlayerCameraController>().SetMoveOutside(true);
        }
    }

    void OnTriggerExit2D(Collider2D other){

        if(other.gameObject.name != "Player"){
            return;
        }

        if(state == DoorState.Optional){
            other.gameObject.GetComponent<PlayerCameraController>().SetMoveOutside(false);
           // Debug.Log("Cant Lol");
        }

        if(state == DoorState.Exit_Left && other.transform.position.x < transform.position.x){
            other.gameObject.GetComponent<PlayerCameraController>().SwitchLayer();
        }
        if(state == DoorState.ExitRight && other.transform.position.x > transform.position.x){
            other.gameObject.GetComponent<PlayerCameraController>().SwitchLayer();
        }
        if(state == DoorState.Exit_Up && other.transform.position.y > transform.position.y + transform.localScale.y/2){
            other.gameObject.GetComponent<PlayerCameraController>().SwitchLayer();
        }
    }
}
