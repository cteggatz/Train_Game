
using System;
using System.Collections.Generic;
using System.IO;
using DataSaving;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject loadSavePanel;
    [SerializeField] private GameControllerInstance gameManager; 

    [SerializeField] private List<SaveController> saves;



    void Start(){
        startPanel.SetActive(true);
        loadSavePanel.SetActive(false);

        string[] saveFiles = FileManager.GetSaves(); 
        foreach(SaveController saveButton in saves){
            saveButton.SetButton(saveFiles);
        }

    }


    public void GetSaves(){
        startPanel.SetActive(false);
        loadSavePanel.SetActive(true);
    }
    public void GetTitleScreen(){
        startPanel.SetActive(true);
        loadSavePanel.SetActive(false);
    }

    public void NewGame(int saveNumber){
        //Debug.ClearDeveloperConsole();
        gameManager.StartGame(saveNumber);
    }
    public void LoadGame(int save){
        Debug.ClearDeveloperConsole();
        gameManager.StartGame(save);
    }
    
    public static void EndGame(){
        Application.Quit();
    }
}
