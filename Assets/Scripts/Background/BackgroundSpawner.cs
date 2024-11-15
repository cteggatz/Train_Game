using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    [SerializeField] private float spawnPos = 40.01f;
    [SerializeField] private float spawnOffSet = 0.6f, y_OffSet;
    [SerializeField] private string ObjectTag;
    private bool orderTracker = true;

    void Start() {
        spawnChunk();
        if (backgroundPrefab.GetComponent<SpriteRenderer>() != null) spawnPos = transform.position.x - spawnOffSet + backgroundPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * backgroundPrefab.GetComponent<Transform>().localScale.x;
        //Debug.Log(transform.position.x);
        //Debug.Log(backgroundPrefab.gameObject.transform.localScale.x);
    }
    private void spawnChunk(){
        //Debug.Log("spawned!");
        GameObject a = Instantiate(backgroundPrefab) as GameObject;
        a.transform.position = new Vector2(spawnPos, y_OffSet);
        a.transform.parent = this.gameObject.transform;
        if(orderTracker){
            a.GetComponent<SortingGroup>().sortingOrder = 1;
        }
        else{
            a.GetComponent<SortingGroup>().sortingOrder = 0;
        }
        orderTracker = !orderTracker;
    }
    void OnTriggerExit2D(Collider2D bc)
    {
        if(bc.tag == ObjectTag)
        {
            spawnChunk();
        }
    }

}
