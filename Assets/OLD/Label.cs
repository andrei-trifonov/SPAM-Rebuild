using UnityEngine;
using System.IO;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

[System.Serializable]


public class Label : MonoBehaviour
{
    public string labelName;
    public List<string> scenarioBlock;
    public List<string> illustrationName;
    public List<string> illustrationRName ;
    public List<Sprite> illustrationImage;
    public List<string> audiosName ;
    public List<AudioClip> audioClips;

    public void PreloadImages()
    {
        foreach (var imageName in illustrationRName)
        {
           
            illustrationImage.Add(Resources.Load<Sprite>("images/" + imageName));
        }
    }
    
    public void PreloadAudio()
    {
        foreach (var audioName in audiosName)
        {
            try
            {
                Debug.Log("music/" + audioName + ".mp3");
                AudioClip clip = (AudioClip)Resources.Load("music/" + audioName);
                if (clip != null)
                {
                    audioClips.Add(clip);
                }
               
            }
            catch (Exception e)
            {
              
                    Debug.Log("Не могу загрузить аудио " + audioName );
            }
           
        }
        
    }

    public Label(string name)
    {
        labelName = name;
    }     
    public void addToBlock(string line)
    {
        scenarioBlock.Add(line);
    }
}