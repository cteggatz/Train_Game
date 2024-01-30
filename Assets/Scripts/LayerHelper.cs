using System;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LayerHelper : MonoBehaviour
{
    public enum TrainLayer{
        Outside_Train = 7,
        Inside_Train = 6,
    }
    public static string LayerToString(TrainLayer _layer){
        if(_layer is TrainLayer.Outside_Train) return "Outside_Train";
        if(_layer is TrainLayer.Inside_Train) return "Inside_Train";
        return "";
    }

    [SerializeField] private TrainLayer layer;
    void Start()
    {
        LayerHelper.SwitchLayers(layer, gameObject);
    }

    // ---- Methods ----

    /**
    <summary>
        Switches the layer of the game object to given layer. Edits colliders and rigid body include and exclude mask to fit the layer.
    </summary>

    <param name="layer">
        The layer that you want to switch to and the layer you want to include in masks
    </param>
    <param name="obj">
        The object that you want to edit the layer, collider, and rigid body of
    </param>
    */
    public static void SwitchLayers(TrainLayer layer, GameObject obj){
        /*
        //selecting layer in int form
        int targetLayer = LayerMask.NameToLayer(LayerToString(layer));
        //selecting layer to turn off in int form
        int otherLayer = (layer is TrainLayer.Outside_Train)? LayerMask.NameToLayer(LayerToString(TrainLayer.Inside_Train)) : LayerMask.NameToLayer(LayerToString(TrainLayer.Outside_Train));
    
        //switching layer of the game object
        obj.layer = targetLayer;


        /*
        This section is a tad confusing
        -------
        what I am doing in the first one is selecting the bit of the layer I want to turn off 0000000010
        Then I am inverting the integer 1111111101 with the invert opperator
        then I am using AND gate to turn it off since (1 & 0) = 0

        kinda the same with the second one.
        I am selecting the bit I want to switch 00000000010
        then I am using an OR gate to turn it on because (1 | 0) = 1
        */
        /*
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if(rb != null){
            rb.includeLayers &= ~(1 << otherLayer);
            rb.includeLayers |= (1 << targetLayer);


            rb.excludeLayers &= ~(1 << targetLayer);
            rb.excludeLayers |= (1 << otherLayer);
        }

        // ---- box collider
        BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        if(boxCollider != null){
            boxCollider.includeLayers &= ~(1 << otherLayer);
            boxCollider.includeLayers |= (1 << targetLayer);

            boxCollider.excludeLayers &= ~(1 << targetLayer);
            boxCollider.excludeLayers |= (1 << otherLayer);
        }
        */
        SwitchLayers(layer, obj, (true, true, true, true));
    }



    /**
    <summary>
    Switches the layer of the game object to given layer. Edits colliders and rigid body include and exclude mask to fit the layer.
    </summary>
    <remarks>
    ** Has options to not edit certain colliders
    </remarks>

    <param name="config">
    a touple with options for all of the edited components
    </param>
    <param name="layer">
        The layer that you want to switch to and the layer you want to include in masks
    </param>
    <param name="obj">
        The object that you want to edit the layer, collider, and rigid body of
    </param>

    */
    public static void SwitchLayers(TrainLayer layer, GameObject obj, (bool editRigidBody, bool editBoxCollider, bool editTileCollider, bool editCompositeCollider) config){
        //selects layer in int form
        int targetLayer = LayerMask.NameToLayer(LayerToString(layer));
        int otherLayer = (layer is TrainLayer.Outside_Train)? LayerMask.NameToLayer(LayerToString(TrainLayer.Inside_Train)) : LayerMask.NameToLayer(LayerToString(TrainLayer.Outside_Train));

        //switching layer of the game object
        obj.layer = targetLayer;

        /*
        This section is a tad confusing
        -------
        what I am doing in the first one is selecting the bit of the layer I want to turn off 0000000010
        Then I am inverting the integer 1111111101 with the invert opperator
        then I am using AND gate to turn it off since (1 & 0) = 0

        kinda the same with the second one.
        I am selecting the bit I want to switch 00000000010
        then I am using an OR gate to turn it on because (1 | 0) = 1
        */

        // ---- rigid body ----
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if(rb != null && config.editRigidBody == true){
            rb.includeLayers &= ~(1 << otherLayer);
            rb.includeLayers |= (1 << targetLayer);


            rb.excludeLayers &= ~(1 << targetLayer);
            rb.excludeLayers |= (1 << otherLayer);
        }


        List<Collider2D> components = new List<Collider2D>();

        BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        if(boxCollider != null && config.editBoxCollider)components.Add(boxCollider);

        TilemapCollider2D tileCollider = obj.GetComponent<TilemapCollider2D>();
        if(tileCollider != null && config.editTileCollider)components.Add(tileCollider);

        CompositeCollider2D compCollider = obj.GetComponent<CompositeCollider2D>();
        if(compCollider != null && config.editCompositeCollider)components.Add(compCollider);

        foreach(Collider2D col in components){
            col.includeLayers &= ~(1 << otherLayer);
            col.includeLayers |= (1 << targetLayer);

            col.excludeLayers &= ~(1 << targetLayer);
            col.excludeLayers |= (1 << otherLayer);
        }

        /*

        // ---- box collider
        BoxCollider2D boxCollider = obj.GetComponent<BoxCollider2D>();
        if(boxCollider != null && config.editBoxCollider == true){
            boxCollider.includeLayers &= ~(1 << otherLayer);
            boxCollider.includeLayers |= (1 << targetLayer);

            boxCollider.excludeLayers &= ~(1 << targetLayer);
            boxCollider.excludeLayers |= (1 << otherLayer);
        }
    
        TilemapCollider2D tileCollider = obj.GetComponent<TilemapCollider2D>();
        if(tileCollider != null){
            tileCollider.includeLayers &= ~(1 << otherLayer);
            tileCollider.includeLayers |= (1 << targetLayer);

            tileCollider.excludeLayers &= ~(1 << targetLayer);
            tileCollider.excludeLayers |= (1 << otherLayer);
        }
        */
    }

    public TrainLayer GetLayer() => this.layer;
}
