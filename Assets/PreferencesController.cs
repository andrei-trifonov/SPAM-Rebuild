using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreferencesController : MonoBehaviour
{
    public string playerPrefsValue;

    public Slider valueSlider;

    public Core gameCore;
    // Start is called before the first frame update
    public bool music;
    public bool text;
    public bool sound;

    private void Start()
    {
        valueSlider.value = PlayerPrefs.GetFloat(playerPrefsValue);
    }

    public void OnValueChanged()
    {
        PlayerPrefs.SetFloat(playerPrefsValue, valueSlider.value);
        if (music)
            gameCore.SetMusicSettings(valueSlider.value);
        if (sound)
            gameCore.SetSoundSettings (valueSlider.value);
        if (text)
            gameCore.SetTextDelay(valueSlider.value);
    }
}
