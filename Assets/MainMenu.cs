using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] List<Animator> CUObjects;
    public void CloseUp(){
    
    foreach (Animator a in CUObjects){
    a.SetBool("CU", true);
    }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
    }
}
