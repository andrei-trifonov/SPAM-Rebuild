using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public GameObject GMC;
    public TextMeshProUGUI Savename;
    public TextMeshProUGUI Savetime;
    public Image Preview;
    public string Savenum;
    public Core m_Core;
    public bool isSaved;
    public GameObject YesnoMenu;
    private void Start()
    {
     Renew();   
    }

    public void Renew()
    {
        Preview.sprite = Resources.Load<Sprite>("images/black.jpg");
        Savename.text = Savenum;
        if (PlayerPrefs.GetInt(Savenum + "isSaved") == 1)
        {
            isSaved = true;
            if (PlayerPrefs.GetString(Savenum + "ScenePreview") != "")
                Preview.sprite = Resources.Load<Sprite>("images/" +PlayerPrefs.GetString(Savenum + "ScenePreview"));
            if (PlayerPrefs.GetString(Savenum + "Savetime") != "")
            {
                Savetime.text = PlayerPrefs.GetString(Savenum + "Savetime");
            }
        }
    }

    public void Load()
    {
        if (isSaved)
        {
            m_Core.Load(Savenum);
            GMC.SetActive(false);
        }
    }

    public void SaveOverride()
    {
        m_Core.Save(Savenum);
        Renew();
        GMC.SetActive(false);
    }

    public void Save()
    {
        Renew();
        if (isSaved)
        {
            YesnoMenu.SetActive(true);
            YesnoMenu.GetComponent<YesnoScreen>().CaptureSave(this);
        }
        else
        {
            m_Core.Save(Savenum);
            Renew();
        }
        
    }
}
