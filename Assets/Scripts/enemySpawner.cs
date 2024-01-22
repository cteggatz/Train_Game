using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject grunt;
    [SerializeField] private Transform furnace;
    private float timer, delayAmount;
    void Start()
    {
        delayAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delayAmount)
        {
            timer = 0f;
            Instantiate(grunt).GetComponent<Igiveup>().target = furnace;
        }
    }
}
