using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    [Header("Item Renderer")]
    [SerializeField] private GameObject itemRenderer;
    [SerializeField] private float itemDistance = 0.5f;

    [SerializeField] private Vector3 offset;

    [Header("Gun")]
    //[SerializeField] private Gun gun;

    [SerializeField] private Gun primary;
    [SerializeField] private Gun secondary;
    [SerializeField] private Gun consumable;

    class ItemData{
        public Item_Template _itemReference {get;}
        public float _maxCoolDown {get;}
        public int _maxUses {get;}
        public int _useIncrment;
        public bool canUse;
        public bool isReloading;
        public float currentCoolDown;
        public int currentUses;

        public ItemData(Item_Template item){
            _itemReference = item;
            _maxCoolDown = item.useCooldown;
            _maxUses = item.numberOfUses;
            _useIncrment = item.useIncrement;

            currentUses = _maxUses;
            currentCoolDown = _maxCoolDown;
            canUse = true;
            isReloading = false;
        }
        public void Update(float dt){
            
            if(isReloading){
                currentCoolDown -= dt;
                if(currentCoolDown <= 0){
                    Debug.Log("done");
                    isReloading = false;
                    canUse = true;
                    currentUses = _maxUses;
                    currentCoolDown = _maxCoolDown;
                }
            }else if(!canUse){
                currentCoolDown -= dt;
                if(currentCoolDown <= 0){
                    canUse = true;
                    currentCoolDown = _maxCoolDown;
                }
            }
        }

        public void IncrementItem(){
            canUse = false;
            currentUses -= _useIncrment;
            if(currentUses <= 0){
                currentCoolDown = _itemReference.rearmTime;
                Debug.Log("Need Reload");
            }
        }
    }

    private ItemData[] EquiptedData = new ItemData[3];
    private int currItemIndex = 0;
    private bool isGrabbing = false;
    void Awake(){
        //subscribing to camera to see when changing layer
        gameObject.GetComponent<PlayerCameraController>().OnLayerChange += (object obj,  PlayerCameraController.LayerChangeArgs e) =>{
            itemRenderer.layer = e.layer;
        };
        //subscribing to grab script to see when item is grabbed
        gameObject.GetComponent<PlayerGrab>().OnGrabInteraction += (object obj, PlayerGrab.GrabArgs e) => {
            isGrabbing = e.isGrabbed;
            if(isGrabbing){
                itemRenderer.GetComponent<SpriteRenderer>().sprite = null;
            } else {
                itemRenderer.GetComponent<SpriteRenderer>().sprite = EquiptedData[currItemIndex]._itemReference.sprite;
            }
        };
        
        EquiptedData[0] = new ItemData(primary);
        EquiptedData[1] = new ItemData(secondary);
        EquiptedData[2] = new ItemData(consumable);
        SetCurrentItem(0);

        offset = transform.GetComponent<PlayerCameraController>().GetCameraOffset() + offset;
        
    }
    
    
    
    void Update()
    {
        //if item is grabbed, nothing happens here.
        if(isGrabbing){
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


        // ---- Inputs ----
        ItemData currentItem = EquiptedData[currItemIndex];
        currentItem.Update(Time.deltaTime);
        //checks for shooting
        if(Input.GetMouseButton(0) && !currentItem.isReloading && currentItem.canUse && currentItem.currentUses > 0){
            currentItem._itemReference.Use(this, angle, offsetPosition, itemDistance, gameObject.layer);
            currentItem.IncrementItem();
        }
        //checks for reload
        if(Input.GetKeyDown(KeyCode.R) && currentItem.currentUses != currentItem._maxUses){
            currentItem.isReloading = true;
            currentItem.currentCoolDown = currentItem._itemReference.rearmTime;
        }

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
    }
    void SetCurrentItem(int index){
        if(index < 0 || index >= EquiptedData.Length){
            Debug.LogError($"Index out of Bounds! [provided index: {index}]");
        }

        currItemIndex = index;
        ItemData currentItem = EquiptedData[currItemIndex];
        itemRenderer.GetComponent<SpriteRenderer>().sprite = currentItem._itemReference.sprite;
        itemRenderer.transform.localScale = currentItem._itemReference.sprite_Size;
    }
}
