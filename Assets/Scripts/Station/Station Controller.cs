using System.Collections;
using System.Collections.Generic;
using DataSaving;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StationController : MonoBehaviour, ISavable
{

    [SerializeField] private TrainCartController train;
    [SerializeField] private Material cartMaterial;
    public void Start(){
        string outDebug = "<color=red>[StationController]</color> setting carts";
        
        void SetChildren(Transform parent){
            outDebug += $"\n {parent.childCount}";
            for(int i = 0; i < parent.childCount; i++){
                Transform child = parent.GetChild(i);

                if(child.childCount > 0){
                    for(int j = 0; j < child.childCount; j++){
                        SetChildren(child.GetChild(j));
                    }
                }

                TilemapRenderer mapRender = child.GetComponent<TilemapRenderer>();
                if(mapRender != null){
                    mapRender.material = cartMaterial;
                }
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if(spriteRenderer != null){
                    spriteRenderer.material = cartMaterial;
                }


                LayerHelper layerHelper = child.GetComponent<LayerHelper>();
                CompositeCollider2D collider = child.GetComponent<CompositeCollider2D>();
                TilemapCollider2D tileCollider = child.GetComponent<TilemapCollider2D>();
                if(tileCollider == false)continue;
                if(collider == null)continue;
                if(layerHelper == null || layerHelper.GetLayer() == LayerHelper.TrainLayer.Inside_Train)continue;

                outDebug += $"Disable Collision Cart {child.name}";
                tileCollider.enabled = false;
                collider.enabled = false;
            }
        }

        foreach(GameObject cart in train.GetCarts()){
            SetChildren(cart.transform);
        }
        Debug.Log(outDebug);



    }


    public void Save(ref GameData data){

    }
    public void Load(ref GameData data){
    }
}
