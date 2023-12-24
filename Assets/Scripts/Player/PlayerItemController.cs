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

    [Header("Gun")]
    [SerializeField] private Gun gun;

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

    private List<ItemData> EquiptedData = new List<ItemData>();
    [SerializeField] private ItemData currentItem;

    // Update is called once per frame
    void Awake(){
        //setting renderer size and shape
        gameObject.GetComponent<PlayerCameraController>().OnLayerChange += (object obj,  PlayerCameraController.LayerChangeArgs e) =>{
            itemRenderer.layer = e.layer;
        };
        AddItem(gun);
        SetCurrentItem(0);
    }
    
    
    
    void Update()
    {
        // ---- Item Positioning Around The Player ----
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(
                transform.position.y - mousePos.y,
                transform.position.x - mousePos.x
        );
        itemRenderer.transform.eulerAngles = new Vector3(0,0,angle * Mathf.Rad2Deg);
        itemRenderer.transform.position = new Vector3(
                    transform.position.x - Mathf.Cos(angle)* itemDistance,
                    transform.position.y - Mathf.Sin(angle)* itemDistance,
                    transform.position.z
        );

        

        
        // ---- Inputs ----
        currentItem.Update(Time.deltaTime);
        if(Input.GetMouseButton(0) && !currentItem.isReloading && currentItem.canUse && currentItem.currentUses > 0){
            currentItem._itemReference.Use(this, angle, transform.position, itemDistance, gameObject.layer);
            currentItem.IncrementItem();
        }
        if(Input.GetKeyDown(KeyCode.R) && currentItem.currentUses != currentItem._maxUses){
            currentItem.isReloading = true;
            currentItem.currentCoolDown = currentItem._itemReference.rearmTime;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            Debug.Log("Switching to Primary");
        }
    }

    public void AddItem(Item_Template item){
        EquiptedData.Add(new ItemData(item));
    }
    void SetCurrentItem(int index){
        if(index < 0 || index >= EquiptedData.Count){
            Debug.LogError($"Index out of Bounds! [provided index: {index}]");
        }

        currentItem = EquiptedData[index];
        itemRenderer.GetComponent<SpriteRenderer>().sprite = currentItem._itemReference.sprite;
        itemRenderer.transform.localScale = currentItem._itemReference.sprite_Size;
    }
}
