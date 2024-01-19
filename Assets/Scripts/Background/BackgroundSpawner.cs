using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    [SerializeField] private float spawnPos = 40.01f;

    void Start(){
        spawnChunk();
        spawnPos = transform.position.x + 0.01f + backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x;
        Debug.Log(transform.position.x);
        Debug.Log(backgroundPrefab.gameObject.transform.localScale.x);
    }
    private void spawnChunk(){
        //Debug.Log("spawned!");
        GameObject a = Instantiate(backgroundPrefab) as GameObject;
        a.transform.position = new Vector2(spawnPos,0.8f);
    }
    void OnTriggerExit2D(Collider2D bc)
    {
        spawnChunk();
        //Debug.Log("Trigger Exit");
    }

}
