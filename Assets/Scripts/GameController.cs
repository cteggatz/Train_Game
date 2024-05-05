using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;
using Unity.VisualScripting;

public class GameControllerInstance : MonoBehaviour, ISavable{


    [SerializeField] public GameDataInstance gameControllerData;
    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;
    [SerializeField] Train_Controller traincontroller;



    // ----- Game Controller Logic ------

    /// <summary>
    /// Initializes <c>Game Controller Instance</c>, Controls when and how the data is loaded.
    /// </summary>
    public void Awake(){
        int scene = SceneManager.GetActiveScene().buildIndex;

        ///if on title screen -> wait to load data until 
        if(scene == 0){
            this.gameControllerData.gameState = GameDataInstance.GameState.Title;
            Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Default Game : {{State : {this.gameControllerData.gameState}}} ");
            return;
        } 
        
        //checks for what the current scene is so logic can function
        if(scene == 1){
            this.gameControllerData.gameState = GameDataInstance.GameState.Train;
        } else if(scene == 2){
            this.gameControllerData.gameState = GameDataInstance.GameState.Station;
        }
        
        //if the game data hasn't been initialized -> do initial reading of disk data, else -> load and save per usual
        if(gameControllerData.initialized == false){
            Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Test Game : {{State : {this.gameControllerData.gameState}}} \n" + "loading into test envoirnment");
            gameControllerData.gameData = SavingManager.Init(0);
            gameControllerData.initialized = true;
            return;
        }
        gameControllerData.gameData = SavingManager.Load();
        SavingManager.Save(gameControllerData.gameData);
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate(){
        switch(gameControllerData.gameState){
            case GameDataInstance.GameState.Train:
                TrainLogic();
                break;
            case GameDataInstance.GameState.Station:
                break;
            case GameDataInstance.GameState.CutScene:
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



    // ----- Logic Helpers ------
    public void StartGame(int saveNumber){
        gameControllerData.gameData = SavingManager.Init(saveNumber);
        gameControllerData.initialized = true;
        SwitchScene(1);
        
        this.gameControllerData.gameState = GameDataInstance.GameState.Train;
    }
    public void SwitchScene(int sceneNumber){
        SavingManager.Save(gameControllerData.gameData);
        SceneManager.LoadScene(sceneNumber);
    }



    // ----- Unity Application Functions ------
    void OnEnable(){SceneManager.activeSceneChanged += OnSceneLoaded;}
    void OnDisable(){SceneManager.activeSceneChanged -=OnSceneLoaded;}
    
    /// <summary>
    /// Responsible for initializing all other game objects on the scene and setting the game manager
    /// </summary>
    /// <param name="oldScene"></param>
    /// <param name="newScene"></param>
    private void OnSceneLoaded(Scene oldScene, Scene newScene){
        Debug.Log($"<color=green>[GameManager]</color> [2] Loading into {newScene.name} & {newScene.buildIndex}");
        
        switch(newScene.buildIndex){
            case 1: //This is the Train Scene
                this.gameControllerData.gameState = GameDataInstance.GameState.Train;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                FindAnyObjectByType<PlayerUIController>().GetComponent<PlayerUIController>().setGameController(this);

                if(gameControllerData.gameData.distance >= gameControllerData.gameData.endDistance){
                    this.distance = 0;
                    this.endDistance = UnityEngine.Random.Range(10f, 50f);
                }
                break;
            case 2: // Station
                this.gameControllerData.gameState = GameDataInstance.GameState.Station;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                break;
            
        }
    }

    private void OnApplicationQuit(){
        Debug.Log($"<color=green>[GameManager]</color> [2] EXITING! Saving");
        SavingManager.Save(gameControllerData.gameData);
        this.gameControllerData.Reset();
    }



    // ----- Getters and Setters ------
    public (float, float) getDistance() => (distance, endDistance);

    public GameDataInstance.GameState GetGameState() => this.gameControllerData.gameState;



    // ----- Saving and Loading Data ------
    public void Save(ref GameData data){
        data.distance = this.distance;
        data.endDistance = this.endDistance;
    }
    public void Load(ref GameData data){
        this.distance = data.distance;
        this.endDistance = data.endDistance;
        gameControllerData.gameData = data;
    }
}

/// <summary>
/// This is a scriptable object instance of the <c>GameData</c> class. This the data to persist between scenes
/// </summary>
[CreateAssetMenu(fileName = "GameController", menuName = "ScriptableObjects/GameController", order = 1), Serializable]
public class GameDataInstance : ScriptableObject
{

    public enum GameState{
        Title,
        Train,
        Station,
        CutScene
    }

    [Header("General Game Manager Data")]
    public bool initialized = false;
    public GameState gameState;

    [Header("Game State")]
    public GameData gameData;

    public void Reset(){
        this.initialized = false;
        this.gameData = null;
        this.gameState = GameState.Title;
    }
}
