using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState{
    Exit_Left,
    ExitRight,
    Optional
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
        }
    }

    void OnTriggerExit2D(Collider2D other){

        if(other.gameObject.name != "Player"){
            return;
        }

        if(state == DoorState.Optional){
            other.gameObject.GetComponent<PlayerCameraController>().SetMoveOutside(false);
        }

        if(state == DoorState.Exit_Left && other.transform.position.x < transform.position.x){
            other.gameObject.GetComponent<PlayerCameraController>().SwitchLayer();
        }
        if(state == DoorState.ExitRight && other.transform.position.x > transform.position.x){
            other.gameObject.GetComponent<PlayerCameraController>().SwitchLayer();
        }
    }
}
