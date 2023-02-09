using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    public LogComposer LogComp;
    public SaveManager LoadMan;
    public GameObject SaveMan;
    public GameObject Preferences;
    public GameObject Menu;

    public void ShowGameMenu(){}

    public void Return()
    {
        LoadMan.gameObject.SetActive(false);
        Menu.SetActive(false);
    }

    public void ShowLog()
    {   
        Preferences.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(false);
        LogComp.gameObject.SetActive(true);
        LogComp.Compose();
    }
    public void ExitToMenu(){}

    public void ShowSaves()
    {
        Preferences.SetActive(false);
        LogComp.gameObject.SetActive(false);
        LoadMan.gameObject.SetActive(false);
        SaveMan.SetActive(true);
    }

    public void ShowLoads()
    {
        Preferences.SetActive(false);
        LogComp.gameObject.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(true);
        LoadMan.RenewSaves();
        
    }
    public void ShowPrefs()
    {
        Preferences.SetActive(true);
        LogComp.gameObject.SetActive(false);
        SaveMan.SetActive(false);
        LoadMan.gameObject.SetActive(false);

    }
}
