using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesController : MonoBehaviour
{


    

    public NewGameCore m_Core;
    // Start is called before the first frame update
    public  Slider  music;
    public  Slider  text;
    public  Slider  sound;
    public  Slider  scene;
    public Toggle textCheck;
    public TMPro.TMP_Dropdown language;
    public bool mainMenu;
    public int languageOld;
    private void SetSettings(int sliderNum)
    {
        switch (sliderNum){
            case 0: m_Core.SetMusicSettings(music.value); break;
            case 1: m_Core.SetSoundSettings(sound.value); break;
            case 2: m_Core.SetSceneAudioSettings(scene.value); break;
            case 3: m_Core.SetTextDelay(text.value); break;
        }

    }
    private void Start()
    {
        music.value = PlayerPrefs.GetFloat("MusicVolume");
        text.value = PlayerPrefs.GetFloat("TextDelay");
        sound.value = PlayerPrefs.GetFloat("SoundVolume");
        scene.value = PlayerPrefs.GetFloat("SceneVolume");
        if (PlayerPrefs.GetFloat("TextDelay") == 0)
        {
            text.value = 0;
            text.interactable = false;
            textCheck.isOn = false;
        }
        else
        {
            textCheck.isOn = true;
        }

        if (PlayerPrefs.GetString("Localization") == "Russian")
        {
            language.value = 0;
            languageOld = 0;
        }
        
        if (PlayerPrefs.GetString("Localization") == "English")
        {
            language.value = 1;
            languageOld = 1;
        }
        if (!mainMenu)
        {
            SetSettings(0);
            SetSettings(1);
            SetSettings(2);
            SetSettings(3);
            
        }
    }

    public void OnMusicValueChanged()
    {
        PlayerPrefs.SetFloat("MusicVolume", music.value);
        if (!mainMenu)
            SetSettings(0);
        PlayerPrefs.Save();
    }
    
    public void OnSoundValueChanged()
    {
        PlayerPrefs.SetFloat("SoundVolume", sound.value);
        if (!mainMenu)
            SetSettings(2);
        PlayerPrefs.Save();
    }
    
    public void OnTextValueChanged()
    {
        if (textCheck.isOn)
        {
           
            PlayerPrefs.SetFloat("TextDelay", text.value);
            if (!mainMenu)
                SetSettings(3);
        }
        PlayerPrefs.Save();
        
    }
    
    public void OnTextCheckValueChanged()
    {
        if (!textCheck.isOn)
        {
            PlayerPrefs.SetFloat("TextDelay", 0);
            text.interactable = false;
            text.value = 0;
        }
        else
        {
            text.interactable = true;
        }

        if (!mainMenu)
            SetSettings(3);
        PlayerPrefs.Save();
    }
    public void OnSceneValueChanged()
    {
        PlayerPrefs.SetFloat("SceneVolume", scene.value);
        if (!mainMenu)
            SetSettings(3);
        PlayerPrefs.Save();
    }
    
    public void OnLangValueChanged()
    {
        Debug.Log(language.ToString() + languageOld.ToString());
        if (language.value == 0)
            PlayerPrefs.SetString("Localization", "Russian");
        if (language.value == 1)
            PlayerPrefs.SetString("Localization", "English");
        if (!mainMenu && languageOld != language.value)
        {
            languageOld = language.value;
            m_Core.ChangeLocalization(language.value);
        }

        PlayerPrefs.Save();
    }
}
