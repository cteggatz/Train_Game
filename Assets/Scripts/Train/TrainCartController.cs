using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class TrainCartController : MonoBehaviour
{
    public GameObject genericTrainCart;
    public GameObject trainHead;
    public GameObject coalCart;

    public GameObject EditorAddedCart;

    public int cartDistance;
    public Vector3 offset;

    private List<GameObject> Carts = new List<GameObject>();




    public void AddCart(GameObject cart){
        for(int i = 0; i < Carts.Count; i++){
            if(Carts[i] == null) Carts.RemoveAt(i);
        }

        GameObject cartInstance = null;
        if(Carts.Count == 0){
            cartInstance = Instantiate(cart);
            cartInstance.transform.SetParent(gameObject.transform);
            cartInstance.transform.localPosition =  offset;
        } else {            
            cartInstance = Instantiate(cart);
            cartInstance.transform.SetParent(gameObject.transform);

            //int xDistance = Mathf.FloorToInt(Carts[Carts.Count-1].transform.localPosition.x) - Mathf.FloorToInt(Carts[Carts.Count-1].GetComponent<CartController>().cartSize.x/2 + cart.GetComponent<CartController>().cartSize.x/2 + (float)cartDistance -1);
            float xDistance = Mathf.Ceil(Carts[Carts.Count-1].transform.localPosition.x) - Mathf.Ceil(Carts[Carts.Count-1].GetComponent<CartController>().cartSize.x/2) - Mathf.Ceil(cart.GetComponent<CartController>().cartSize.x/2) - cartDistance + 1;
            //Debug.Log($"POS : {Mathf.FloorToInt(Carts[Carts.Count-1].transform.localPosition.x)} | Distance : {xDistance}");
            cartInstance.transform.localPosition = new Vector3(
                    xDistance, 
                    offset.y,
                    offset.z
            );    
        }
        cartInstance.GetComponent<CartController>().SetCart(gameObject);
        Carts.Add(cartInstance);

    }

    private void Awake(){
        
        for(int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).GetComponent<CartController>().SetCart(gameObject);
            Carts.Add(transform.GetChild(i).gameObject);
        }

        AddCart(coalCart);
        AddCart(genericTrainCart);
        
        
        for(int i = 0; i < Carts.Count; i++){
            if(Carts[i] == null) Carts.RemoveAt(i);
        }
    }
}

[CustomEditor(typeof(TrainCartController))]
public class TrainCartControllerGUI : Editor{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        TrainCartController cartController = (TrainCartController)target;

        cartController.genericTrainCart = (GameObject) EditorGUILayout.ObjectField("generic cart", cartController.genericTrainCart, typeof(GameObject), true);
        cartController.coalCart = (GameObject) EditorGUILayout.ObjectField("coal cart", cartController.coalCart, typeof(GameObject), true);
        cartController.trainHead = (GameObject) EditorGUILayout.ObjectField("train head", cartController.trainHead, typeof(GameObject), true);

        cartController.cartDistance = EditorGUILayout.IntField("Cart Distance", cartController.cartDistance);
        Vector3 newOffset = EditorGUILayout.Vector3Field("Offset", cartController.offset);
        cartController.offset = new Vector3(
            Mathf.Floor(newOffset.x),
            Mathf.Floor(newOffset.y),
            Mathf.Floor(newOffset.z)
        );

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        EditorGUILayout.LabelField("Add and Remove Carts", EditorStyles.boldLabel);
        cartController.EditorAddedCart = (GameObject) EditorGUILayout.ObjectField("Prefab To Add", cartController.EditorAddedCart, typeof(GameObject), true);

        if(GUILayout.Button("Add Cart") == true && cartController.EditorAddedCart != null){
            cartController.AddCart(cartController.EditorAddedCart);
        }

    }
}
