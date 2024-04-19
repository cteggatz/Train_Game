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
    public enum GameState{
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

            int scene = SceneManager.GetActiveScene().buildIndex;
            if(scene == 0){
                this.gameState = GameState.Title;
                Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Default Game : {{State : {this.gameState}}} ");
            } else {
                if(scene == 1){
                    this.gameState = GameState.Train;
                } else if(scene == 2){
                    this.gameState = GameState.Station;
                }
                Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Test Game : {{State : {this.gameState}}} \n" + "loading into test envoirnment");
                SavingManager.Init(0);
                SavingManager.Load();
                SavingManager.Save();
            }
            this.initialized = true;
            
        } else {
            SavingManager.Load();
            Destroy(this.gameObject);
        }
        
    }
    
    private void OnSceneLoaded(Scene oldScene, Scene newScene){
        Debug.Log($"<color=green>[GameManager]</color> [2] Loading into {newScene.name} & {newScene.buildIndex}");
        
        switch(newScene.buildIndex){
            case 1: //This is the Train Scene
                this.gameState = GameState.Train;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                FindAnyObjectByType<PlayerUIController>().GetComponent<PlayerUIController>().setGameController(this);
        
                break;
            case 2: // Station
                this.gameState = GameState.Station;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                break;
            
        }
    }


    public void StartGame(int saveNumber){
        SavingManager.Init(saveNumber);
        SwitchScene(1);
        this.gameState = GameState.Train;
    }

    public void SwitchScene(int sceneNumber){
        SavingManager.Save();
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

    public GameState GetGameState() => this.gameState;

    public void Save(ref GameData data){
        data.distance = distance;
        data.endDistance = endDistance;
    }
    public void Load(ref GameData data){}
}

