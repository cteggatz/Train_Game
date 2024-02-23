using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class spawnOnTrain : MonoBehaviour
{
    public Vector3 spawn;
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        float smooth = speed * Time.deltaTime;
        if(transform.position.x != spawn.x){
            transform.position = new Vector3(transform.position.x + smooth, transform.position.y, transform.position.z);
        }
        if(transform.position.y != spawn.y){
            transform.position = new Vector3(transform.position.x, transform.position.y  + smooth, transform.position.z);
        }
        if(transform.position.x >= spawn.x){
            gameObject.transform.GetChild(0).GetComponent<Igiveup>().target = GameObject.FindGameObjectWithTag("Player").transform;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            gameObject.transform.GetChild(0).parent = null;
            Destroy(gameObject);
        }
    }
}
