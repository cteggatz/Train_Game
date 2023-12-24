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

    class ItemData{
        public Item_Template _itemReference {get;}
        public float _maxCoolDown {get;}
        public int _maxUses {get;}
        public int _useIncrment;
        public bool canUse;
        public bool needReload;
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
            needReload = false;
        }
        public void Update(float dt){
            //Debug.Log($"{currentCoolDown}");
            if(currentUses <= 0 && !needReload){
                currentCoolDown = _itemReference.rearmTime;
            }
            if(needReload){
                currentCoolDown -= dt;
                if(currentCoolDown <= 0){
                    Debug.Log("done");
                    needReload = false;
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
        //Debug.Log(Time.deltaTime);
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

        currentItem.Update(Time.deltaTime);

        //shoot
        if(Input.GetMouseButton(0) && !currentItem.needReload && currentItem.canUse){
            currentItem._itemReference.Use(this, angle, transform.position, itemDistance, gameObject.layer);
            currentItem.canUse = false;
            currentItem.currentUses -= currentItem._useIncrment;
            if(currentItem.currentUses <= 0)Debug.Log("Need Reload");
        }
        if(Input.GetKeyDown(KeyCode.R) && currentItem.currentUses != currentItem._maxUses){
            Debug.Log(currentItem);
            Debug.Log("reloading!");
            currentItem.needReload = true;
            currentItem.currentCoolDown = currentItem._itemReference.rearmTime;
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
