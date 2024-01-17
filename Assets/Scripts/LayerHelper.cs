using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerHelper : MonoBehaviour
{
    enum TrainLayer{
        Outside_Train,
        Inside_Train,
    }
    private string LayerToString(TrainLayer _layer){
        if(_layer is TrainLayer.Outside_Train) return "Outside_Train";
        if(_layer is TrainLayer.Inside_Train) return "Inside_Train";
        return "";
    }

    [SerializeField] TrainLayer layer;
    void Start()
    {
        int targetLayer = LayerMask.NameToLayer(LayerToString(layer));

        int otherLayer = (layer is TrainLayer.Outside_Train)? LayerMask.NameToLayer(LayerToString(TrainLayer.Inside_Train)) : LayerMask.NameToLayer(LayerToString(TrainLayer.Outside_Train));

        gameObject.layer = targetLayer;

        Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
        if(rb != null){
               
        }

        BoxCollider2D boxCollider = transform.GetComponent<BoxCollider2D>();
        if(boxCollider != null){

        }
    }

}
