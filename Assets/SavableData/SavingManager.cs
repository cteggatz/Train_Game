using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEditor;
using GameItems;
using System.IO;
using UnityEngine.Windows;
using UnityEditor.PackageManager;
using Unity.VisualScripting;

namespace DataSaving{
    public class SavingManager : MonoBehaviour
    {
        static GameData currentData;
        public static void Save(){
            //Debug.Log("Saving!");
            GameData gameData = currentData;
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
        }

        public static void Load(){
            //Debug.Log("Loading!");
            /*
            GameData gameData = FileManager.Load();
            if(gameData == null){
                gameData = new GameData();
            }
            */
            GameData gameData;
            if(currentData != null){
                gameData = currentData;
            } else {
                gameData = FileManager.Load();
                if(gameData == null){gameData = new GameData();}
            }
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();
            foreach(ISavable script in savableObjects){
                script.Load(ref gameData);
            }
            currentData = gameData;
        }

        public static void Init(int saveNumber){
            FileManager.Init(saveNumber);

            string[] saveFiles = FileManager.GetSaves();
            foreach(string save in saveFiles){
                if(Path.GetFileName(save).Equals($"SaveData{saveNumber}.json")){
                    //Load();
                    return;
                }
            }
            CreateNewGame();

        }  
        public static void CreateNewGame(){
            Debug.LogWarning("<color=yellow>[FileManager]</color> No File! Creating Save File");
            GameData gameData = new GameData();
            IGameInit[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IGameInit>().ToArray();

            foreach(IGameInit script in savableObjects){
                script.Init(ref gameData);
            }
            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
            currentData = gameData;
        }



    }

    /// --------
    /// <summary>
    /// Class Responsible for converting given Game Data into a json and saving it to the disk
    /// </summary>
    public class FileManager{
        private static int saveNumber;
        static string jsonData;

        public static void Init(int saveNumber){
            FileManager.saveNumber = saveNumber;
            Debug.Log($"<color=yellow>[FileManager]</color> Initialized File Manager | [Pointing to save : SaveFile{saveNumber}.json]");
        }
        public static void Save(GameData data){
            jsonData = JsonUtility.ToJson(data, true);
            //Debug.Log(AssetDatabase.GetAssetPath(SavingManager.Manager) + " | " + Application.persistentDataPath);
            System.IO.File.WriteAllText(Application.persistentDataPath + $"/SaveData{saveNumber}.json", jsonData);
        }
        public static GameData Load(){
            try{
                jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + $"/SaveData{saveNumber}.json");
                return JsonUtility.FromJson<GameData>(jsonData);
            } catch(Exception e){
                Debug.LogError($"<color=yellow>[FileManager]</color><color=red><Error></color> SaveFile{saveNumber} not found | {e.ToShortString()}");
                System.IO.File.WriteAllText(Application.persistentDataPath + $"/SaveData{saveNumber}.json", "");
                return null;
            }
        }

        public static string[] GetSaves(){
            //Debug.Log($"Retrieving Save Data | {System.IO.Directory.GetFiles(Application.persistentDataPath).Length}");
            return System.IO.Directory.GetFiles(Application.persistentDataPath);
        }
    }

 

    /// <summary>
    /// Class Repesentation of the Data in the Game
    /// </summary>
    [Serializable]
    public class GameData{

        // ----- Data -----
        // initialization
        public bool playerInitialized = false;
        public bool trainInitialized = false;

        // train
        public float fuel;
        public SerializableList<CartData> carts;

        //game
        public float distance;
        public float endDistance;

        //player
        public int playerHealth;
        public SerializableList<GunData> playerGuns;


        // ----- functions -----
        public GameData(){
            carts = new SerializableList<CartData>();
            playerGuns = new SerializableList<GunData>();
        }
        public override string ToString()
        {
            string returnString = $"Game Data [Distance : {this.distance}] | [endDistance : {this.endDistance}] \n Train Data [Carts : {this.carts.list.ToString()}] \nPlayer Data {this.playerGuns.ToString()}";
            return returnString + base.ToString();
        }
        public void SaveCart(GameObject cart, int index){
            if(index >= this.carts.list.Count){
                this.carts.list.Add(new GameData.CartData(cart.GetComponent<CartController>().prefabReference));
            } else {
                carts.list[index] = new GameData.CartData(cart.GetComponent<CartController>().prefabReference);
            }
        }
        public void SaveGun(ItemInstance item, int index){
            if(index >= this.playerGuns.list.Count){
                playerGuns.list.Add(new GunData(item.ammo, item.reference));
            } else {
                playerGuns.list[index] = new GunData(item.ammo, item.reference);
            }
        }
    
        // classes
        [Serializable]
        public class SerializableList<T> {
            public List<T> list;

            public SerializableList(){
                list = new List<T>();
            }
        }
        [Serializable]
        public class CartData{
            public string Address;
            public CartData(string address){
                Address = address;
            }
        }
        [Serializable]
        public struct GunData{
            public int ammo;
            public string reference;
            public GunData(int ammo, Usable_Item item){
                this.ammo = ammo;
                this.reference = AssetDatabase.GetAssetPath(item);
            }
        }
    }
    public interface ISavable{
        public void Save(ref GameData gamedata);
        public void Load(ref GameData gamedata);
    }
    public interface IGameInit{
        public void Init(ref GameData gameData){}
    }
}
