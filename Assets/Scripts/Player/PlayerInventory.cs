using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using GameItems;
using DataSaving;
using UnityEditor;
using Unity.VisualScripting.Dependencies.NCalc;

/// <summary>
/// This class is responsible for organizing the inventory of the player and rendering it out
/// </summary>
public class PlayerInventory : MonoBehaviour, ISavable
{
    //---- Item Settings ----
    [Header("Items")]
    //item references which we will then turn into item instances
    [SerializeField] private Usable_Item _primary;
    [SerializeField] private Usable_Item _secondary;
    [SerializeField] private Usable_Item _consumable;

     // ---- Grab Script ----
     /*
        This needs to change later! I am not going to change how items like fuel works, but we need a new system layer! -Chris
     */
    [Header("Other Item Grabbing")]
    [Tooltip("Point where the ray is casted from")] [SerializeField] private Transform rayPoint;
    [Tooltip("Point where a grabed object will teleport")] [SerializeField] private Transform grabPoint;
    [SerializeField]  private GameObject coalPrefab,  grabbedObject; //... the current grabbed object
    [SerializeField] private float rayDistance; //This should be self explanitory
    private bool isGrabbing;
    public bool canCreatCoal;

    [SerializeField] private GameObject CoalSpawner;


    //---- item rendering ----
    [Header("Item Renderer")]
    [SerializeField] private GameObject itemRenderer;
    [SerializeField] private float itemDistance = 0.5f;
    [SerializeField] private Vector3 offset;

    [SerializeField] private GameObject groundedItemPrefab;


    //---- Actual Inventory ----
    private ItemInstance[] inventory = new ItemInstance[4];
    private int currentItem;

    void Start()
    {
        //creating instances of the item data fed. 
        //inventory[0] = new ItemInstance(_primary);
        //inventory[1] = new ItemInstance(_secondary);
        //inventory[2] = new ItemInstance(_consumable);
        SetCurrentItem(0);

        gameObject.GetComponent<PlayerCameraController>().OnLayerChange += (object obj,  PlayerCameraController.LayerChangeArgs e) =>{
            itemRenderer.layer = e.layer;
            if(isGrabbing){
                /**
                This is basically switching the layers the collider includers when switching layers
                */
                switch(LayerMask.LayerToName(e.layer)){
                    case "Outside_Train":
                        LayerHelper.SwitchLayers(LayerHelper.TrainLayer.Outside_Train, grabbedObject);
                        break;
                    case "Inside_Train":
                        LayerHelper.SwitchLayers(LayerHelper.TrainLayer.Inside_Train, grabbedObject);
                        break;
                }
            }
        };
        //offset = transform.GetComponent<PlayerCameraController>().GetCameraOffset();

        isGrabbing = false;
    }

    void Update()
    {

        //---- Item Grabbing ----
        /*
        We need to change this later but this will do. This is basically just the grab script inside this script.
        Its litterally two different systems in one and we need to change that.

        We do the grab first because we dont want the item to render if the item is picked up.
        */
        if(Input.GetKey("f") && !isGrabbing)
        {
            if(canCreatCoal){
                grabbedObject = CoalSpawner.GetComponent<CoalSpawner>().SpawnCoal();
            } else {
                //casts ray at foot to see if then can grab anything
                RaycastHit2D hitinfo = Physics2D.Raycast(rayPoint.position, transform.right, rayDistance);
                if(hitinfo.collider == null || hitinfo.collider.gameObject.layer != gameObject.layer)return;


                //checks if the object is one of the grabbable things, if not then function will return;
                if(hitinfo.collider.gameObject.tag == "Fuel") { 
                    grabbedObject = hitinfo.collider.gameObject; 
                } 
                else if(hitinfo.collider.gameObject.name == "Grabbable - Spawner") { 
                    grabbedObject = Instantiate(coalPrefab); 
                }
                else return;
            }
            grabbedObject.GetComponent<Rigidbody2D>().isKinematic = true;
            grabbedObject.transform.position = grabPoint.position;
            grabbedObject.transform.SetParent(transform);

            isGrabbing = true;
            itemRenderer.transform.localScale = Vector3.zero;
        }else if ( isGrabbing && (Input.GetKeyUp(KeyCode.F) == true || grabbedObject == null)){
            //if item exits, gets rid of it and puts it on ground
            if(grabbedObject != null){
                grabbedObject.GetComponent<Rigidbody2D>().isKinematic = false;
                grabbedObject.transform.SetParent(null);
                grabbedObject = null;
            }
            isGrabbing = false;
            itemRenderer.transform.localScale = inventory[currentItem].reference.sprite_Size;
        } else if(isGrabbing){
            return;
        }



        // ---- Item Positioning Around The Player ----
        //accounts for camera offset
        Vector3 offsetPosition = new Vector3(
            transform.position.x +offset.x,
            transform.position.y +offset.y,
            transform.position.z +offset.z
        );
        //gets mouse pos and get the angle
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
                offsetPosition.y - mousePos.y,
                offsetPosition.x- mousePos.x
        );
        //moves item into the right position relative to the mouse
        itemRenderer.transform.position = new Vector3(
                    offsetPosition.x - Mathf.Cos(angle)* itemDistance,
                    offsetPosition.y- Mathf.Sin(angle)* itemDistance,
                    offsetPosition.z
        );

        // ---- Rotation and textures ----
        //rotates renderer
        itemRenderer.transform.eulerAngles = new Vector3(0,0,angle * Mathf.Rad2Deg);
        //flips texture if past 90 degrees
        if(Mathf.Abs(itemRenderer.transform.eulerAngles.z - 180) > 90){
            itemRenderer.GetComponent<SpriteRenderer>().flipY = false;
        } else {
            itemRenderer.GetComponent<SpriteRenderer>().flipY = true;
        } 


        // ---- Inputs ---
        //switching guns
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            Debug.Log("Switching to Primary");
            SetCurrentItem(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            Debug.Log("Switching to Secondary");
            SetCurrentItem(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            Debug.Log("Switching to Consumable");
            SetCurrentItem(2);
        }

        //Using Guns
        if(Input.GetKeyDown(KeyCode.R)){
            inventory[currentItem].Reload(transform);
        }
        if(Input.GetMouseButtonDown(0)){
            inventory[currentItem].Use(transform, offsetPosition, angle, itemRenderer.layer);
        }

        if(Input.GetKeyDown(KeyCode.C)){
            GameObject item = Instantiate(groundedItemPrefab);
            if(item.GetComponent<Grounded_Item>() != null){
                item.GetComponent<Grounded_Item>().SetItem(inventory[currentItem], (LayerHelper.TrainLayer)gameObject.layer);
            }
        }
 
    }



    void SetCurrentItem(int index){
        if(index < 0 || index >= inventory.Length){
            Debug.LogError($"Index out of Bounds! [provided index: {index}]");
        }

        currentItem = index;

        itemRenderer.GetComponent<SpriteRenderer>().sprite = inventory[index].reference.sprite;
        itemRenderer.transform.localScale = inventory[index].reference.sprite_Size;
    }

    public ItemInstance GetCurrentItem(){
        return inventory[currentItem];
    }

    private void OnTriggerEnter2D(Collider2D collider){
        if(collider.GetComponent<CoalSpawner>() != null){
            CoalSpawner = collider.gameObject;
        }
    }

    public void Save(ref GameData data){
        List<GameData.GunData> gunDatas = new List<GameData.GunData>();
        foreach(ItemInstance item in inventory){
            if(item != null && item.reference != null){
                gunDatas.Add(new GameData.GunData(item.ammo, item.reference));
            }
        }
        data.playerGuns.list = gunDatas;
    }
    public void Load(ref GameData data){
        if(data.playerInitialized == false){
            Debug.Log("No Player Data - Initializing Player Data");
            inventory[0] = new ItemInstance(_primary);
            inventory[1] = new ItemInstance(_secondary);
            inventory[2] = new ItemInstance(_consumable);
            inventory[3] = null;
            data.playerInitialized = true;
            return;
        }
        Debug.Log("Loading Player Data");
        void setItem(int index, ref GameData data){
            if(data == null || data.playerGuns.list[index].reference == null){
                inventory[index] = new ItemInstance(_primary);
                return;
            }
            inventory[index] = new ItemInstance(AssetDatabase.LoadAssetAtPath<Usable_Item>(data.playerGuns.list[index].reference));
            inventory[index].ammo = data.playerGuns.list[index].ammo;
            Debug.Log($"Loading gun {index} | [{inventory[index]}]");
        }
        setItem(0, ref data);
        setItem(1, ref data);
        setItem(2, ref data);
    }

}
