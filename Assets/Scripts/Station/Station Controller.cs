using System.Collections;
using System.Collections.Generic;
using DataSaving;
using UnityEngine;

public class StationController : MonoBehaviour, ISavable
{
    
    private List<GameObject> parkedTrain = new List<GameObject>();

    public void Start(){

        foreach(GameObject cart in parkedTrain){
            foreach(Transform child in cart.transform){
                SpriteRenderer sprite = child.GetComponent<SpriteRenderer>();
                Collider2D collider = child.GetComponent<Collider2D>();

                if(sprite != null){
                    sprite.sortingOrder = 2;
                }
                if(collider != null){
                    collider.enabled = false;
                }
            }
        }



    }


    public void Save(ref GameData data){

    }
    public void Load(ref GameData data){
        
        foreach(GameData.CartData cart in data.carts.list){
            for(int i = 0; i < parkedTrain.Count; i++){
                if(parkedTrain[i] == null) parkedTrain.RemoveAt(i);
            }
            parkedTrain.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(cart.Address));
        }
    }
}
