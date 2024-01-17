using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_Controller : MonoBehaviour
{
    
    [SerializeField, Range(0,100)] float speed = 0;
    [SerializeField, Min(0)] float fuel = 100;

    
    


    [Header("Speed Settings")]

    /**<summary>The degress at which the train speed accelerates when the fuel threshold is met</summary>*/
    [SerializeField, Min(0f)] private float acceleration = 1f;

    /**<summary>The degree to which the train slows down when the threshhold is not met. Basically air resistance coefficient</summary>*/
    [SerializeField, Range(0f,1f)] private float deceleration = .99f;

    /**<summary>The speed in MPH which the train is attempting to reach</summary>*/
    [SerializeField, Range(0,100)] float targetSpeed = 50f;


    [Header("Fuel Settings")]
    /**<summary>The minimum fuel before the train starts to slow down</summary>*/
    [SerializeField, Range(0, 50f)]private float fuelThreshold;

    /**<summary>The coefficient multiplied by the sqrt(speed) when fuel is being decrimented</summary>*/
    [SerializeField] float burnCoEfficient = 1f;



    void FixedUpdate()
    {
    /*
        The speed function is changing in terms of time
        fuel is decrimenting abritrary to the Fuel

        If you are above the fuel threshhold, the speed will increase, almost linearly but there is an asmpytote at the target speed.

     */
     if (fuel >= fuelThreshold) {
        speed += increment(speed,acceleration * Time.deltaTime); 
     } else {
        speed *= Mathf.Pow(deceleration, Time.deltaTime);
     }
     //fuel -= .5f * Time.deltaTime;
     if(fuel >0){
        fuel -= burnCoEfficient * Mathf.Sqrt(speed) * Time.deltaTime;
     } else if(fuel < 0){
        fuel = 0;
     }
    }

    private float increment(float speed, float deltaSpeed){
        return (targetSpeed - speed) * (1f-Mathf.Exp(-deltaSpeed/targetSpeed));
    }

    public float GetSpeed(){
        return this.speed;
    }
    public float GetFuel(){
        return this.fuel;
    }

    public void AddFuel(float fuel){
        this.fuel += fuel;
    }

}