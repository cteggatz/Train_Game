using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public static void StartGame(){
        SceneManager.LoadScene(1);
    }
    public static void EndGame(){
        Application.Quit();
    }
}
