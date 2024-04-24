using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DataSaving;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Controls the train in the main scene
/// </summary>
public class TrainCartController : MonoBehaviour, ISavable
{
    public GameObject genericTrainCart;
    public GameObject trainHead;
    public GameObject coalCart;

    public GameObject EditorAddedCart;

    public int cartDistance;
    public Vector3 offset;

    [SerializeField] private List<GameObject> Carts = new List<GameObject>();



    /// <summary>
    /// Adds cart prefab to train's known carts
    /// </summary>
    /// <param name="cart">the cart prefab that will be added to the trian</param>
    public GameObject AddCart(GameObject cart){
        //Debug.Log("[TrainController] Adding Cart");
        for(int i = 0; i < Carts.Count; i++){
            if(Carts[i] == null) Carts.RemoveAt(i);
        }

        GameObject cartInstance = Instantiate(cart);

        if(PrefabUtility.IsPartOfPrefabAsset(cart)){
            cartInstance.GetComponent<CartController>().SetCart(gameObject, cart, true);
        } else {
            cartInstance.GetComponent<CartController>().SetCart(gameObject, cart);
        }

        if(Carts.Count == 0){
            cartInstance.transform.SetParent(gameObject.transform);
            cartInstance.transform.localPosition =  offset;
            Debug.Log(offset);
        } else {            
            cartInstance.transform.SetParent(gameObject.transform);

            //int xDistance = Mathf.FloorToInt(Carts[Carts.Count-1].transform.localPosition.x) - Mathf.FloorToInt(Carts[Carts.Count-1].GetComponent<CartController>().cartSize.x/2 + cart.GetComponent<CartController>().cartSize.x/2 + (float)cartDistance -1);
            float xDistance = Mathf.Ceil(Carts[Carts.Count-1].transform.localPosition.x) - Mathf.Ceil(Carts[Carts.Count-1].GetComponent<CartController>().cartSize.x/2) - Mathf.Ceil(cart.GetComponent<CartController>().cartSize.x/2) - cartDistance + 1;
            cartInstance.transform.localPosition = new Vector3(xDistance, offset.y,offset.z);    
        }
        Carts.Add(cartInstance);
        return cartInstance;
    }
    public void setCarts(){
        for(int i = 0; i < transform.childCount; i++){
            GameObject.Destroy(transform.GetChild(i).gameObject);            
        }
    }


    private void Awake(){
        /*
        for(int i = 0; i < transform.childCount; i++){
            //GameObject exCart = transform.GetChild(i).gameObject; 
            //exCart.GetComponent<CartController>().SetCart(gameObject, exCart, true);
            //Destroy(transform.GetChild(i).gameObject);
        }*/
        
        //AddCart(coalCart);
        //AddCart(genericTrainCart);
        
        for(int i = 0; i < Carts.Count; i++){
            if(Carts[i] == null) {
                Carts.RemoveAt(i);
            } else {
                //Debug.Log($"Instance Information | name : {Carts[i].cart.name} | reference : {Carts[i].cart.GetComponent<CartController>().prefabReference}");
            }
        }
    }


    public void Save(ref GameData data){
        List<GameData.CartData> cartDataList = new List<GameData.CartData>();
        
        foreach(GameObject cart in Carts){
            //data.SaveCart(cart);
            cartDataList.Add(new GameData.CartData(cart.GetComponent<CartController>().prefabReference));
        }
        data.carts.list = cartDataList;
    }
    public void Load(ref GameData data){
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        this.Carts = new List<GameObject>();

        if(data.trainInitialized == false){
            Debug.Log("[TrainController] No Cart Data - Initializing Train Data");
            AddCart(trainHead);
            AddCart(coalCart);
            AddCart(genericTrainCart);
            data.trainInitialized = true;
            return;
        }
        foreach(GameData.CartData cart in data.carts.list){
            AddCart(AssetDatabase.LoadAssetAtPath<GameObject>(cart.Address));
        }
        
        hoardLogic HoardController = FindAnyObjectByType<hoardLogic>();
        if(HoardController != null){
            HoardController.SetTargets(this.Carts);
        }
        //Debug.Log("Loaded Train Carts");

    }

    public ref List<GameObject> GetCarts(){
        return ref this.Carts;
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
