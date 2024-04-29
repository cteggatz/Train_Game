using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataSaving;
using System;
using Unity.VisualScripting;


/**
This is the main controller of the game

This utilizes a singleton class structure seeing as this is a persistant game object.
For some reason a new gameManager game object is always created so the structure goes a little like this.

1) Awake() -> instantiates new instance of game manager on new scene. Deleates if no instance. Loads and initates loading manager

2) OnSceneLoad() -> effectivly the awake function for instances of GameController Instance. All scene spesific things are done here on awake

3) Normal Stuff.
*/
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

    private Dictionary<int, float> tripLog = new Dictionary<int, float>(); 


    [SerializeField, Min(0)] float distance = 0;
    [SerializeField, Min(0)] float endDistance = 0;

    [SerializeField] Train_Controller traincontroller;


    void OnEnable(){SceneManager.activeSceneChanged += OnSceneLoaded;}
    
    void OnDisable(){SceneManager.activeSceneChanged -=OnSceneLoaded;}


    void Awake(){
        //new instance
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
        } else { // run if instance already exists
            SavingManager.Load();
            Destroy(this.gameObject);
        }
        
    }
    
    //essentially Awake function 
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
            tripLog.Add(tripLog.Count, distance);
            SwitchScene(2);
        }
    }

    // ---- getters and setters
    public (float, float) getDistance() => (distance, endDistance);

    public GameState GetGameState() => this.gameState;

    
    public void Save(ref GameData data){
        data.distance = distance;
        data.endDistance = endDistance;
        data.tripLog.list.Clear();
        foreach(var kvp in tripLog){
            Debug.Log(kvp);
            data.tripLog.list.Add(new GameData.SerializableKeyValuePair<int, float>(kvp.Key, kvp.Value));
        }
        //re-work trip log to be a dictionary that converts into a array of key value pairs that converts to JSON.
    }
    public void Load(ref GameData data){
        
    }
}

