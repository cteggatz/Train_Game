using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    /**
    <summary>
    This class is responsible for organizing the inventory of the player and rendering it out
    </summary>
    */
    //---- Item Settings ----
    [Header("Items")]
    //item references which we will then turn into item instances
    [SerializeField] private Usable_Item _primary;
    [SerializeField] private Usable_Item _secondary;
    [SerializeField] private Usable_Item _consumable;

    //---- item rendering ----
    [Header("Item Renderer")]
    [SerializeField] private GameObject itemRenderer;
    [SerializeField] private float itemDistance = 0.5f;
    [SerializeField] private Vector3 offset;

    //---- Actual Inventory ----
    private ItemInstance[] inventory = new ItemInstance[4];
    private int currentItem;
    private class ItemInstance{
        public Usable_Item reference;
        int ammo;
        bool onUseCooldown;
        bool reloading;
        
        
        public ItemInstance(Usable_Item reference){
            this.reference = reference;
            ammo = reference.maxUseQuantity;
            onUseCooldown = false;
            reloading = false;
        }

        public void Use(Transform playerPos, Vector3 pos, float angle, int layer){
            if(!onUseCooldown && !reloading){
                reference.use(playerPos, pos, angle, layer);
                ammo -= reference.useIncrement;
                

                onUseCooldown = true;
                if(ammo > 0){
                    playerPos
                        .transform
                        .GetComponent<PlayerInventory>()
                        .StartCoroutine(ItemCooldown());
                }
            }
        }
        private IEnumerator ItemCooldown(){
            yield return new WaitForSeconds(reference.useCooldown);
            onUseCooldown = false;
        }
        public void Reload(Transform player){
            if(reloading){return;}
            reloading = true;
            if(reference is New_Gun_Template){
                New_Gun_Template _gun = (New_Gun_Template) reference;
                player
                    .transform
                    .GetComponent<PlayerInventory>()
                    .StartCoroutine(ReloadWithDelay(_gun.reloadSpeed));
            } else {
                player
                    .transform
                    .GetComponent<PlayerInventory>()
                    .StartCoroutine(ReloadWithDelay(0));
            }
        }
        private IEnumerator ReloadWithDelay(float time){
            yield return new WaitForSeconds(time);
            onUseCooldown = false;
            ammo = reference.maxUseQuantity;
            reloading = false;
        }
    }

    
    


    void Start()
    {
        //creating instances of the item data fed. 
        inventory[0] = new ItemInstance(_primary);
        inventory[1] = new ItemInstance(_secondary);
        inventory[2] = new ItemInstance(_consumable);
        SetCurrentItem(0);

        gameObject.GetComponent<PlayerCameraController>().OnLayerChange += (object obj,  PlayerCameraController.LayerChangeArgs e) =>{
            itemRenderer.layer = e.layer;
        };
        offset = transform.GetComponent<PlayerCameraController>().GetCameraOffset();
    }

    void Update()
    {
        //itemRenderer.transform.rotation = Quaternion.Euler (0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
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

        if(Input.GetMouseButtonDown(0)){
            inventory[currentItem].Use(transform, offsetPosition, angle, itemRenderer.layer);
        }
        if(Input.GetKeyDown(KeyCode.R)){
            inventory[currentItem].Reload(transform);
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
}
