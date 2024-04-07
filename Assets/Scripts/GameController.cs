using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;
using Unity.VisualScripting;

public class GameControllerInstance : MonoBehaviour, ISavable
{
    public bool initialized = false;
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

    
    /**
    New Archicture proposal. 

    [] No persistant instance game manager!

    Save Game State -> Load new Scene -> load state in new instance of game manager


    */

    void OnEnable(){SceneManager.activeSceneChanged += OnSceneLoaded;}
    
    void OnDisable(){SceneManager.activeSceneChanged -=OnSceneLoaded;}

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            this.gameState = GameState.Title;
            Debug.Log($"---- [1] Instantiating Game : {{State : {this.gameState}}} ----");
            initialized = true;
        } else {
            Destroy(this.gameObject);
        }
    }
    
    private void OnSceneLoaded(Scene oldScene, Scene newScene){
        Debug.Log($"---- [2] Loading into {newScene.name} ----");
        if(newScene.buildIndex != 2){
            SavingManager.Load();
        }
        switch(newScene.buildIndex){
            case 1: //This is the Train Scene
                this.gameState = GameState.Train;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                FindAnyObjectByType<PlayerUIController>().GetComponent<PlayerUIController>().setGameController(this);
                break;
            case 2:
                this.gameState = GameState.Title;
                break;
            
        }
    }


    public void StartGame(int saveNumber){
        SavingManager.Init(saveNumber);
        SwitchScene(1);
        this.gameState = GameState.Train;
        SavingManager.InitializeGameObjects();
    }

    public void SwitchScene(int sceneNumber){
        SavingManager.Save();
        Debug.Log("Saving Scene");
        SceneManager.LoadScene(sceneNumber);
    }




    void FixedUpdate(){
        switch(gameState){
            case GameState.Train:
                TrainLogic();
                break;
            case GameState.Station:
                break;
            case GameState.CutScene:
                break;
            default:
                break;
        }
    }

    private void TrainLogic(){
        distance += Time.deltaTime * traincontroller.GetSpeed() / 60f;

        if(distance >= endDistance){
            SwitchScene(2);
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

