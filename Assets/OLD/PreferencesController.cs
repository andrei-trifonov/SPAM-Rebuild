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

    private void Start()
    {
        valueSlider.value = PlayerPrefs.GetFloat(playerPrefsValue);
        if (music)
            m_Core.SetMusicSettings(valueSlider.value);
        if (sound)
            m_Core.SetSoundSettings(valueSlider.value);
        if (text)
            m_Core.SetTextDelay(valueSlider.value);
    }

    public void OnValueChanged()
    {
        PlayerPrefs.SetFloat(playerPrefsValue, valueSlider.value);
        if (music)
            m_Core.SetMusicSettings(valueSlider.value);
        if (sound)
            m_Core.SetSoundSettings (valueSlider.value);
        if (text)
            m_Core.SetTextDelay(valueSlider.value);
    }
}
