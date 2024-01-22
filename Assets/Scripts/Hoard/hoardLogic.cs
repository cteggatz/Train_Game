using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class hoardLogic : MonoBehaviour
{
    private float timer;
    [SerializeField]  private float delayAmount;
    [SerializeField] private GameObject grunt;
    [SerializeField] private Vector2 location;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delayAmount)
        {
            timer = 0f;
            spawnGrunt(0, location, 4, 10);
        }
    }

    void spawnGrunt(int target, Vector2 spawn, float awareness, float health)
    {
        GameObject lad = Instantiate(grunt);
        lad.GetComponent<Igiveup>().target = targets[target];
        lad.GetComponent<Igiveup>().awareness = awareness;
        lad.GetComponent<Igiveup>().health = health;
        lad.transform.position = spawn;
    }

    void spawnOnTrain()
    {
        Vector2 spawn = new Vector2(0.0f, 0.0f);
    }
}
