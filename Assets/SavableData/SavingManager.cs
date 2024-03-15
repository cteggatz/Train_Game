using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEditor;

namespace DataSaving{
    public class SavingManager : MonoBehaviour
    {
        public static SavingManager Manager {get; private set;}

        void Awake(){
            if(SavingManager.Manager != null){
                SavingManager.Manager = this;
            }
            //PopulateRegistry();
            Save();
            Load();
        }


        public void Save(){
            GameData gameData = new GameData();
            //Debug.Log($"game data {gameData}");
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();

            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            //Debug.Log(gameData.carts.list.Count);
            FileManager.Save(gameData);
        }

        public void Load(){
            Debug.Log(
                FileManager.Load().ToString()
            );
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
            //Debug.Log(jsonData);
        }
        public static GameData Load(){
            return JsonUtility.FromJson<GameData>(jsonData);
        }
    }

 

    /// <summary>
    /// Class Repesentation of the Data in the Game
    /// </summary>
    [Serializable]
    public class GameData{
        public GameData(){
            carts = new SerializableCartsList<CartData>();
        }
        public override string ToString()
        {
            string returnString = $" [Distance : {this.distance}] | [endDistance : {this.endDistance}] | [Carts : {this.carts.list.ToString()}]";
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

        public SerializableList<CartData> carts;

        //Game Data
        public float distance;
        public float endDistance;


        public void SaveCart(GameObject cart){
            this.carts.list.Add(new GameData.CartData(cart.GetComponent<CartController>().prefabReference));
        }

        // ----- Player ------
        public int playerHealth;
        public struct gunData{

        }
        public SerializableList<gunData> playerGuns;


    }
    public interface ISavable{
        public void Save(ref GameData gamedata);
        public void Load();
    }
}
