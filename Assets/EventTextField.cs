using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class EventTextField : MonoBehaviour
{

    private NewGameCore Core;
   [SerializeField] private GameObject Clicker;
   [SerializeField] private TMP_Text Text;
   private void OnEnable()
    {
       
        Core = GameObject.FindObjectOfType<NewGameCore>();
        Core.SetQTE(true);
    }

    public void OnValueChanged()
    {
        Debug.Log(Text.text); 
        
        if ((Text.text == "Я хочу, чтобы Соня была жива​") || (Text.text == "I want Sonya to be aliveZWSP"))
        {
            Core.jumpAction("День4 Секрет");
            
        }  
      
        
            
        Core.SetQTE(false);
        Core.EnableText(true);
        Core.Step();
    
        gameObject.SetActive(false);
    }
}
