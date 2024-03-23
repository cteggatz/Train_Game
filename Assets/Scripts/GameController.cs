using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;

public class GameController : MonoBehaviour, ISavable
{
    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;

    [SerializeField] Train_Controller traincontroller;

    


    void Awake(){
        if(SavingManager.Load() == false){
            NewGame();
        }
    }

    void NewGame(){
        SavingManager.Init();
    }
    void FixedUpdate(){
        distance += Time.deltaTime * traincontroller.GetSpeed() / 60f;

        if(distance >= endDistance){
            SavingManager.Save();
            SavingManager.Load();
            SceneManager.LoadScene(0);
        }
    }

    

    public (float, float) getDistance() => (distance, endDistance);

    public void Save(ref GameData data){
        data.distance = distance;
        data.endDistance = endDistance;
    }
    public void Load(ref GameData data){}
}

