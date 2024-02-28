using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.Unicode;

public class hoardLogic : MonoBehaviour
{
    private float timer, gametime;
    [SerializeField]  private float delayAmount, runtime, runUpSpeed;
    [SerializeField, Range(0, 100)] private float difficulty;
    [SerializeField] private GameObject grunt, side_grunt;
    [SerializeField] private Vector2 H_location;
    [SerializeField] private Vector2 spawnTarget;
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
            if(random <= 0){
                spawnOnTrain();
            }
            else{
                spawnGrunt(0, H_location, Random.Range(4, 6), (int) (10 * random));
            }
        }
        
    }

    void spawnGrunt(int target, Vector2 spawn, float awareness, float health)
    {
        GameObject lad = Instantiate(grunt);
        lad.layer = 6;
        if (Random.Range(1, 3) > 2)
        {
            lad.layer = 7;
        }
        lad.transform.SetParent(gameObject.transform);
        lad.GetComponent<Igiveup>().target = targets[target];
        lad.GetComponent<Igiveup>().awareness = awareness;
        lad.GetComponent<Igiveup>().health = health;
        lad.transform.position = spawn;
    }

    private void spawnOnTrain()
    {
        float smooth = runUpSpeed * Time.deltaTime;
        GameObject sideman = Instantiate(side_grunt);
        sideman.transform.SetParent(gameObject.transform);
        
        spawnGrunt(0, spawnTarget, Random.Range(4, 6), (int)(10));
    }

}
