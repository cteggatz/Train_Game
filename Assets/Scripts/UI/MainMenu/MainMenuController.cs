
using DataSaving;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuController : MonoBehaviour
{
    
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject loadSavePanel;
    [SerializeField] private GameControllerInstance gameManager; 


    void Start(){
        startPanel.SetActive(true);
        loadSavePanel.SetActive(false);

    }


    public void GetSaves(){
        startPanel.SetActive(false);
        loadSavePanel.SetActive(true);
    }

    public void NewGame(){
        gameManager.StartGame(FileManager.GetSaves().Length);
    }
    public void LoadGame(int save){
        gameManager.StartGame(save);
    }
    
    public static void EndGame(){
        Application.Quit();
    }
}
