using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] List<Animator> CUObjects;
    [SerializeField] private GameObject introObject; 
    private void Awake()
    {
        if (FindObjectOfType<SingletoneForIntro>().intro)
           introObject.SetActive(false);
    }

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
