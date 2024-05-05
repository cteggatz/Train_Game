using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;
using Unity.VisualScripting;

/*
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

                this.distance = 0;
                this.endDistance = UnityEngine.Random.Range(10f, 50f);

        
                break;
            case 2: // Station
                this.gameState = GameState.Station;
                traincontroller = FindAnyObjectByType<Train_Controller>().GetComponent<Train_Controller>();
                break;
            
        }
    }


    public void StartGame(int saveNumber){
        SavingManager.Init(saveNumber);
        SavingManager.Load();
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
*/

public class GameControllerInstance : MonoBehaviour, ISavable{


    [SerializeField] public GameDataInstance gameControllerData;
    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;
    [SerializeField] Train_Controller traincontroller;

    void OnEnable(){SceneManager.activeSceneChanged += OnSceneLoaded;}
    void OnDisable(){SceneManager.activeSceneChanged -=OnSceneLoaded;}

    public void Awake(){
        int scene = SceneManager.GetActiveScene().buildIndex;
        if(scene == 0){
            this.gameControllerData.gameState = GameDataInstance.GameState.Title;
            Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Default Game : {{State : {this.gameControllerData.gameState}}} ");
            return;
        } 

        if(scene == 1){
            this.gameControllerData.gameState = GameDataInstance.GameState.Train;
        } else if(scene == 2){
            this.gameControllerData.gameState = GameDataInstance.GameState.Station;
        }
        
        if(gameControllerData.initialized == false){
            Debug.Log($"<color=green>[GameManager]</color> [1] Instantiating Test Game : {{State : {this.gameControllerData.gameState}}} \n" + "loading into test envoirnment");
            gameControllerData.gameData = SavingManager.Init(0);
            gameControllerData.initialized = true;
            return;
        }
        gameControllerData.gameData = SavingManager.Load();
        SavingManager.Save(gameControllerData.gameData);
    }

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


    private void OnApplicationQuit(){
        Debug.Log($"<color=green>[GameManager]</color> [2] EXITING! Saving");
        SavingManager.Save(gameControllerData.gameData);
        this.gameControllerData.Reset();
    }

    // ---- getters and setters
    public (float, float) getDistance() => (distance, endDistance);

    public GameDataInstance.GameState GetGameState() => this.gameControllerData.gameState;


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
