using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class WheelController : MonoBehaviour
{
    [SerializeField] private Transform transform;

    void Start()
    {
        transform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        var rot = transform.localEulerAngles;
        rot.z += -Time.deltaTime * 100;
        if(rot.z <= -360){
            rot.z = 0;
        }
        transform.localEulerAngles = rot;   
    }
}
