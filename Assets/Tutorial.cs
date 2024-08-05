using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject RU_text;

    public GameObject EN_text;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Localization")=="English")
        {
            EN_text.SetActive(true);
            RU_text.SetActive(false);
        }
        else
        {
            EN_text.SetActive(false);
            RU_text.SetActive(true);
        }

        GetComponent<Canvas>().worldCamera = GameObject.Find("investigationCamera").GetComponent<Camera>();
    }


}
