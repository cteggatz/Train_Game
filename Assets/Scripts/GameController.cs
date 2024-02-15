using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;

    [SerializeField] Train_Controller traincontroller;

    void FixedUpdate(){
        distance += Time.deltaTime * traincontroller.GetSpeed() / 60f;

        if(distance >= endDistance){
            SceneManager.LoadScene(0);
        }
    }

    

    public (float, float) getDistance() => (distance, endDistance);
}
