using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameItems;

public class PlayerUIController : MonoBehaviour
{
    [Header("Player Gun Text")]
    [SerializeField] private TMP_Text playerItemInfo;
    [SerializeField] private TMP_Text playerHealthText;
    [SerializeField] private GameObject player;
    [Header("Player Health")]

    [Header("Train Text")]
    [SerializeField] private TMP_Text trainFuel;
    [SerializeField] private TMP_Text trainHealth;
    [SerializeField] private TMP_Text distance;

    [SerializeField] private GameObject trainController;
    [SerializeField] private GameObject gameController;
     


    // Update is called once per frame
    void Update()
    {
        ItemInstance  item = player.GetComponent<PlayerInventory>().GetCurrentItem();
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        Train_Controller trainControllerInstance = trainController.GetComponent<Train_Controller>();




        if(item.reloading){
            playerItemInfo.text = "reloading...";
        } else if(item.ammo ==0){
            playerItemInfo.text = $"{item.reference.name} : [Ammo : <color=red>{item.ammo}</color> / {item.reference.maxUseQuantity}]";
        }else {
            playerItemInfo.text = $"{item.reference.name} : [Ammo : {item.ammo} / {item.reference.maxUseQuantity}]";
        }

        trainFuel.text = $"Fuel : {(int)trainControllerInstance.GetFuel()} | Speed : {(float)Mathf.Round(trainControllerInstance.GetSpeed()*10) * .1f}";
        trainHealth.text = $"Engine Health : [{trainControllerInstance.Health}/{trainControllerInstance.MaxHealth}]";
        playerHealthText.text = $"Health : {health.health}";
        (float, float) trainDistance = gameController.GetComponent<GameControllerInstance>().getDistance();
        distance.text = $"Distance : [{(int)(trainDistance.Item1/trainDistance.Item2 * 1000)/10f}%]";
    }
}
