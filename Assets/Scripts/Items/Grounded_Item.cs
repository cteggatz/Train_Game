using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using GameItems;
using System.Runtime.CompilerServices;

namespace GameItems
{
    public class ItemInstance{
        public Usable_Item reference;
        public int ammo {get; private set;}
        public bool onUseCooldown {get; private set;}
        public bool reloading {get; private set;}
        
        
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
    
    [RequireComponent(typeof(SpriteRenderer))]
    public class Grounded_Item : MonoBehaviour
    {
        [SerializeField] private ItemInstance item;
        
        public void SetItem(ItemInstance item, LayerHelper.TrainLayer layer){
            this.item = item;

            gameObject.GetComponent<SpriteRenderer>().sprite = item.reference.sprite;

            gameObject.GetComponent<BoxCollider2D>().size = item.reference.sprite.bounds.size;

            LayerHelper.SwitchLayers(layer, gameObject);
        }

    }
}
