using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMenuController : MonoBehaviour
{
    public Animator Anim;
    public LogComposer LogComp;
    public SaveManager LoadMan;
    public GameObject SaveMan;
    public GameObject Preferences;
    public GameObject Menu;

    public Canvas Preloader;

    public void ShowGameMenu(){}

    public void Return()
    {
        
        LoadMan.gameObject.SetActive(false);
        Menu.SetActive(false);
    }

    public void ShowLog()
    {   
        Anim.SetBool("Fold", true);
        Preferences.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(false);
        LogComp.gameObject.SetActive(true);
        LogComp.Compose();
    }
    public void ExitToMenu()
    {
        Preloader.enabled = true;
        SceneManager.LoadSceneAsync("MainMenuScene");
    }

    public void ShowSaves()
    {
        Anim.SetBool("Fold", true);
        Preferences.SetActive(false);
        LogComp.gameObject.SetActive(false);
        LoadMan.gameObject.SetActive(false);
        SaveMan.SetActive(true);
    }

    public void ShowLoads()
    {
        Anim.SetBool("Fold", true);
        Preferences.SetActive(false);
        LogComp.gameObject.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(true);
        LoadMan.RenewSaves();
        
    }
    public void ShowPrefs()
    {
        Anim.SetBool("Fold", true);
        Preferences.SetActive(true);
        LogComp.gameObject.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(false);

    }

    public void Fold()
    {
        if (Anim.GetBool("Fold")){
            Anim.SetBool("Fold", false);
        Preferences.SetActive(false);
        LogComp.gameObject.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(true);
        LoadMan.gameObject.SetActive(false);
    }
        else {
            Return();
        
        }
        }
}
