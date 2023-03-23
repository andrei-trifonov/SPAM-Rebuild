using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLaunchSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.GetInt("NotFirstLaunch") == 0)
        {
            PlayerPrefs.SetInt("NotFirstLaunch", 1);
            PlayerPrefs.SetFloat("MusicVolume", 1);
            PlayerPrefs.SetFloat("SoundVolume", 1);
            PlayerPrefs.SetFloat("TextDelay", 0.1f);
        }
    }


}
