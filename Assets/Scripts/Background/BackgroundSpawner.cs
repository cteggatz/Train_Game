using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    private float spawnPos = 50f;
    //private BoxCollider2D bc;

    void Start(){
        //bc = this.GetComponent<BoxCollider2D>();
        spawnChunk();
    }
    private void spawnChunk(){
        Debug.Log("spawned!");
        GameObject a = Instantiate(backgroundPrefab) as GameObject;
        a.transform.position = new Vector2(spawnPos, -1);
    }
    void OnTriggerExit(Collider backGroundBoxCollider)
    {
        spawnChunk();
        Debug.Log("Trigger Exit");
    }

}
