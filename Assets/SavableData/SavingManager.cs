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

namespace DataSaving{
    public class SavingManager : MonoBehaviour
    {


        public static void Save(){
            GameData gameData = new GameData();
            //Debug.Log($"game data {gameData}");
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            //Debug.Log(gameData.carts.list.Count);
            FileManager.Save(gameData);
        }

        public static void Load(){
            GameData gameData = FileManager.Load();
            if(gameData == null){
                Debug.Log("No File! Creating Save File");
                return;
            }
            Debug.Log($"Loading Data : {gameData.ToString()}");
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Load(ref gameData);
            }
        }
    }

    /// --------
    /// <summary>
    /// Class Responsible for converting given Game Data into a json and saving it to the disk
    /// </summary>
    public class FileManager{
        static string jsonData;
        public static void Save(GameData data){
            jsonData = JsonUtility.ToJson(data, true);
            //Debug.Log(AssetDatabase.GetAssetPath(SavingManager.Manager) + " | " + Application.persistentDataPath);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", jsonData);
        }
        public static GameData Load(){
            try{
                jsonData = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
                return JsonUtility.FromJson<GameData>(jsonData);
            } catch(Exception e){
                System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", "");
                return null;
            }
        }
    }

 

    /// <summary>
    /// Class Repesentation of the Data in the Game
    /// </summary>
    [Serializable]
    public class GameData{
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
}
