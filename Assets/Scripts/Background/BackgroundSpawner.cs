using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    private float spawnPos = 40.1f;

    void Start(){
        spawnChunk();
    }
    private void spawnChunk(){
        Debug.Log("spawned!");
        GameObject a = Instantiate(backgroundPrefab) as GameObject;
        a.transform.position = new Vector2(spawnPos, -1);
    }
    void OnTriggerExit2D(Collider2D bc)
    {
        spawnChunk();
        Debug.Log("Trigger Exit");
    }

}
