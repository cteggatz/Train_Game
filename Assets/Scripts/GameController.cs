using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;

public class GameControllerInstance : MonoBehaviour, ISavable
{
    private static GameControllerInstance instance;
    private enum GameState{
        Title,
        Train,
        Station,
        CutScene
    }

    private GameState gameState;


    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;

    [SerializeField] Train_Controller traincontroller;

    


    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            this.gameState = GameState.Title;
            Debug.Log($"---- [1] Instantiating Game : {{State : {this.gameState}}} ----");
        } else {
            Destroy(gameObject);
        }
        //if(SavingManager.Load() == false){NewGame();}
    }

    public void StartGame(int saveNumber){
        SavingManager.Init(saveNumber);
        Debug.Log("---- [2] Loading into Train Scene ----");
        SceneManager.LoadScene(1);
        this.gameState = GameState.Train;
        SavingManager.InitializeGameObjects();
    }




    void FixedUpdate(){
        switch(gameState){
            case GameState.Train:
                TrainLogic();
                break;
        }
    }

    private void TrainLogic(){
        distance += Time.deltaTime * traincontroller.GetSpeed() / 60f;

        if(distance >= endDistance){
            SavingManager.Save();
            SavingManager.Load();
            SceneManager.LoadScene(0);
        }
    }

    // ---- getters and setters
    public (float, float) getDistance() => (distance, endDistance);

    public void Save(ref GameData data){
        data.distance = distance;
        data.endDistance = endDistance;
    }
    public void Load(ref GameData data){}
}

