using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    public GameMenuController GMC;
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

        ResourceRequest request = Resources.LoadAsync<Sprite>("Previews/"+ line);

        while (!request.isDone)
        {
            yield return null;
        }
                   
        if (request.asset == null)
        {
            Debug.LogError("Failed to load priview at path: Previews/" + line);
            request = Resources.LoadAsync<Sprite>("CG/"+ line);
            
            while (!request.isDone)
            {
                yield return null;
            }
                       
            if (request.asset == null)
            {
                Debug.LogError("Failed to load preview at path: CG/" + line);
            }
            else
            {
                Sprite sprite = request.asset as Sprite;
                Preview.sprite = sprite;
            }
        }
        else
        {
            Sprite sprite = request.asset as Sprite;
            Preview.sprite = sprite;
        }

      
       
    }


    public void Renew()
    {
        Savename.text = Savenum;
        if (PlayerPrefs.GetInt(Savenum + "isSaved") == 1)
        {
            SaveObject loadedData = JsonUtility.FromJson<SaveObject>(PlayerPrefs.GetString(Savenum+"Save"));
            isSaved = true;
            if (loadedData.previewName!="")
                 StartCoroutine(LoadPreview(loadedData.previewName));
            Savetime.text = loadedData.Savetime;
        }
    }

    public void Load()
    {
        if (isSaved)
        {
            m_Core.Load(Savenum);
            GMC.Fold();
            GMC.Return();
        }
    }

    public void SaveOverride()
    {
        m_Core.Save(Savenum);
        Renew();
        GMC.Fold();
        GMC.Return();
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
