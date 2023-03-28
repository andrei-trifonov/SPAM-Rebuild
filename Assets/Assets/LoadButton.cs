using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public GameObject GMC;
    public TextMeshProUGUI Savename;
    public TextMeshProUGUI Savetime;
    public Image Preview;
    public string Savenum;
    public NewGameCore m_Core;
    public bool isSaved;
    public GameObject YesnoMenu;
    private void Start()
    {
     Renew();   
    }

    IEnumerator LoadPreview(string line)
    {

        Debug.Log(line);
        AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(line);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite res = handle.Result;
            Preview.sprite = res;
        }

        Addressables.Release(handle);
    }


    public void Renew()
    {
        Savename.text = Savenum;
        if (PlayerPrefs.GetInt(Savenum + "isSaved") == 1)
        {
            isSaved = true;
            if (PlayerPrefs.GetString(Savenum + "Scene") != "")
                 StartCoroutine(LoadPreview(PlayerPrefs.GetString(Savenum + "Scene")+"Preview"));
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
