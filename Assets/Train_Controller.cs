using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_Controller : MonoBehaviour
{
    
    [SerializeField, Range(0,100)] float speed = 0;
    [SerializeField, Range(0,100)] float targetSpeed = 50f;
    [SerializeField, Min(0)] float fuel = 0;



    void Start()
    {
        
    }

    void FixedUpdate()
    {


        if(fuel > 0){
            if(speed <= targetSpeed){
                speed = speed + .1f;
            }   
            fuel -= Time.deltaTime;
        } else {
            speed -= .5f;
        }
    }

    void UpdateSpeed(float speed){
        this.speed += speed;
    }
    public float GetSpeed(){
        return this.speed;
    }

}
