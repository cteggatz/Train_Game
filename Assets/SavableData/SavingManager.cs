using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DataSaving{
    public class SavingManager : MonoBehaviour
    {
        public static SavingManager Manager {get; private set;}

        void Awake(){
            if(SavingManager.Manager != null){
                SavingManager.Manager = this;
            }
            Save();
            Load();
        }

        public void Save(){
            GameData gameData = new GameData();
            ISavable[] savableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISavable>().ToArray();
            foreach(ISavable script in savableObjects){
                script.Save(ref gameData);
            }
            FileManager.Save(gameData);
        }
        public void Load(){
            Debug.Log(
                FileManager.Load()
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
            jsonData = JsonUtility.ToJson(data);
            Debug.Log(jsonData);
        }
        public static GameData Load(){
            return JsonUtility.FromJson<GameData>(jsonData);
        }
    }

    /// <summary>
    /// Class Repesentation of the Data in the Game
    /// </summary>
    public class GameData{
        
        // ----- Train ----
        public float fuel;
        public List<CartData> carts = new List<CartData>();
        public class CartData{
            int prefabNumber;
            public CartData(int prefabNumber){
                this.prefabNumber = prefabNumber;
            }
        }

        public void SaveCart(){

        }
    }
    public interface ISavable{
        public void Save(ref GameData gamedata);
        public void Load();
    }
}
