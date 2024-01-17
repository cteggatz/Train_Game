using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D other){
        if(other.transform.name == "Player"){
            other.GetComponent<Movementv2>().SetCanClimb(true);
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.transform.name == "Player"){
            other.GetComponent<Movementv2>().SetCanClimb(false);
        }
    }
}
