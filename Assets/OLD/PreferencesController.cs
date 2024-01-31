using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesController : MonoBehaviour
{
    public string playerPrefsValue;

    public Slider valueSlider;

    public NewGameCore m_Core;
    // Start is called before the first frame update
    public bool music;
    public bool text;
    public bool sound;
    public bool scene;

    private void SetSettings()
    {
        if (music)
            m_Core.SetMusicSettings(valueSlider.value);
        if (sound)
            m_Core.SetSoundSettings(valueSlider.value);
        if (scene)
            m_Core.SetSceneAudioSettings(valueSlider.value);
        if (text)
            m_Core.SetTextDelay(valueSlider.value);
    }
    private void Start()
    {
        valueSlider.value = PlayerPrefs.GetFloat(playerPrefsValue);
        SetSettings();
    }

    public void OnValueChanged()
    {
        PlayerPrefs.SetFloat(playerPrefsValue, valueSlider.value);
        SetSettings();
    }
}
