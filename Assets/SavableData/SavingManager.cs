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
        public static void Save(){
            GameData gameData = new GameData();
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
        }

        public static bool Load(){
            GameData gameData = FileManager.Load();
            if(gameData == null){
                return false;
            }
            Debug.Log($"Loading Data : {gameData.ToString()}");
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Load(ref gameData);
            }
            return true;
        }

        public static void Init(int saveNumber){
            FileManager.Init(saveNumber);
            if(saveNumber >= FileManager.GetSaves().Length){
                CreateNewGame();
            } else {
                Load();
            }
        }  
        public static void InitializeGameObjects(){
            Debug.Log("Initializing GameObject Data");
            GameData gameData = FileManager.Load();
            IGameInit[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IGameInit>().ToArray();
            Debug.Log($"Items : {savableObjects.Length}");
            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
        }
        public static void CreateNewGame(){
            Debug.Log("No File! Creating Save File");
            GameData gameData = new GameData();
            IGameInit[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IGameInit>().ToArray();

            foreach(IGameInit script in savableObjects){
                script.Init(ref gameData);
            }
            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
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
            Debug.Log($"Initialized File Manager | [Pointing to save : {saveNumber}]");
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
                Debug.Log($"[Error] Save file not found | {e.ToShortString()}");
                System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData{saveNumber}.json", "");
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

        public bool playerInitialized = false;
        public bool trainInitialized = false;

        public GameData(){
            carts = new SerializableList<CartData>();
            playerGuns = new SerializableList<GunData>();
        }
        public override string ToString()
        {
            string returnString = $"Game Data [Distance : {this.distance}] | [endDistance : {this.endDistance}] \n Train Data [Carts : {this.carts.list.ToString()}] \nPlayer Data {this.playerGuns.ToString()}";
            return returnString + base.ToString();
        }

        // ----- Train ----
        public float fuel;
    
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

        public void SaveCart(GameObject cart) => this.carts.list.Add(new GameData.CartData(cart.GetComponent<CartController>().prefabReference));
        public SerializableList<CartData> carts;

        //Game Data
        public float distance;
        public float endDistance;


        // ----- Player ------
        public int playerHealth;
        [Serializable]
        public struct GunData{
            public int ammo;
            public string reference;
            public GunData(int ammo, Usable_Item item){
                this.ammo = ammo;
                this.reference = AssetDatabase.GetAssetPath(item);
            }
        }
        public void SaveGun(ItemInstance item){
            playerGuns.list.Add(new GunData(item.ammo, item.reference));
        }
        public SerializableList<GunData> playerGuns;


    }
    public interface ISavable{
        public void Save(ref GameData gamedata);
        public void Load(ref GameData gamedata);
    }
    public interface IGameInit{
        public void Init(ref GameData gameData){}
    }
}
