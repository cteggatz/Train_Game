using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class hoardLogic : MonoBehaviour
{
    private float timer, gametime;
    [SerializeField]  private float delayAmount, runtime;
    [SerializeField, Range(0, 100)] private float difficulty;
    [SerializeField] private GameObject grunt;
    [SerializeField] private Vector2 H_location;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        gametime += Time.deltaTime;
        if (gametime >= 1)
        {
            gametime = 0;
            runtime += 1;
            difficulty = Mathf.Sqrt(runtime);
            delayAmount = (Mathf.Sqrt(difficulty) * 12.4f) / difficulty;
        }
        if (timer >= delayAmount)
        {
            timer = 0f;
            float random = Random.Range(0.5f, 1.5f);
            Vector3 size = new Vector3(random, random, random);
            spawnGrunt(0, H_location, Random.Range(4, 6), (int) (10 * random), size);
        }
        
    }

    void spawnGrunt(int target, Vector2 spawn, float awareness, float health, Vector3 size)
    {
        GameObject lad = Instantiate(grunt);
        lad.transform.SetParent(gameObject.transform);
        lad.GetComponent<Igiveup>().target = targets[target];
        lad.GetComponent<Igiveup>().awareness = awareness;
        lad.GetComponent<Igiveup>().health = health;
        lad.GetComponent<Igiveup>().size = size;
        lad.transform.position = spawn;
    }

    void spawnOnTrain()
    {
        Vector2 spawn = new Vector2(0.0f, 0.0f);
    }
}
