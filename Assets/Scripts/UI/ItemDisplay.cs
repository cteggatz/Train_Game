using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemDisplay : MonoBehaviour
{
    
    private TMP_Text text;
    [SerializeField] private GameObject player;
    
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        /*
        PlayerInventory.ItemInstance item = player.GetComponent<PlayerInventory>().GetCurrentItem();
        if(item.reloading){
            text.text = "reloading...";
        } else if(item.ammo ==0){
            text.text = $"{item.reference.name} : [Ammo : <color=red>{item.ammo}</color> / {item.reference.maxUseQuantity}]";
        }else {
            text.text = $"{item.reference.name} : [Ammo : {item.ammo} / {item.reference.maxUseQuantity}]";
        }
        */
    }
}
