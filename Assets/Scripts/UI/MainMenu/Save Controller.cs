using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SaveController : MonoBehaviour 
{
    private Button button;
    private Button deleteSaveButton;
    private TMP_Text text;
    
    [SerializeField] int saveNumber;

    void Awake(){
        button = gameObject.GetComponent<Button>();
        deleteSaveButton = gameObject.GetComponentInChildren<Button>();
        text = gameObject.GetComponentInChildren<TMP_Text>();

        button.onClick.AddListener(delegate{FindObjectOfType<GameControllerInstance>().StartGame(saveNumber);});
        deleteSaveButton.gameObject.SetActive(false);
    }

    public void SetButton(string[] saves){
        foreach(string save in saves){
            if(Path.GetFileName(save).Contains($"{saveNumber}")){
                text.text = $"Load Save [{saveNumber}]";
                deleteSaveButton.gameObject.SetActive(true);
                return;
            }
        }
    }
}
