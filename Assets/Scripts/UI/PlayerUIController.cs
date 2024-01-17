using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("Player Gun Text")]
    [SerializeField] private TMP_Text playerItemInfo;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject player;
    [Header("Player Health")]

    [Header("Train Text")]
    [SerializeField] private TMP_Text trainFuel;
    [SerializeField] private GameObject trainController;
     


    // Update is called once per frame
    void Update()
    {
        PlayerInventory.ItemInstance item = player.GetComponent<PlayerInventory>().GetCurrentItem();
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        Train_Controller trainControllerInstance = trainController.GetComponent<Train_Controller>();


        if(item.reloading){
            playerItemInfo.text = "reloading...";
        } else if(item.ammo ==0){
            playerItemInfo.text = $"{item.reference.name} : [Ammo : <color=red>{item.ammo}</color> / {item.reference.maxUseQuantity}]";
        }else {
            playerItemInfo.text = $"{item.reference.name} : [Ammo : {item.ammo} / {item.reference.maxUseQuantity}]";
        }

        trainFuel.text = $"Fuel : {trainControllerInstance.GetFuel()} | Speed : {trainControllerInstance.GetSpeed()}";
        playerHealthText.text = $"Health : {health.health}";
    }
}
