using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public float spawnTime = 1.0f;
    private float spawnPos = 30f;
    void Start()
    {
        StartCoroutine(chunkSpawner());
    }
    private void spawnChunk(){
        GameObject a = Instantiate(backgroundPrefab) as GameObject;
        a.transform.position = new Vector2(spawnPos, -1);
    }
    IEnumerator chunkSpawner(){
        while(true){
            yield return new WaitForSeconds(spawnTime);
            spawnChunk();
        }
    }
}
