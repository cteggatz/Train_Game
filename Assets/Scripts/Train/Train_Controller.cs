using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_Controller : MonoBehaviour
{
    
    
    [SerializeField, Range(0,100)] float speed = 0;
    [SerializeField, Range(0,100)] float targetSpeed = 50f;

    
    private float timer;
    [SerializeField] private float delayAmount;
    [SerializeField, Min(0)] float fuel = 0;



    void Start()
    {
        
    }

    void Update()
    {
       timer += Time.deltaTime;
        if (timer >= delayAmount)
        {
            timer = 0f;
            if(fuel > 0){
                fuel -= 0.5f;
            }
        }
    }

    void UpdateSpeed(float speed){
        this.speed += speed;
    }
    public void AddSpeed(float fuel){
        this.fuel += fuel;
    }
    public float GetFuel(){return this.fuel;}
    public float GetSpeed(){return this.speed;}
}
