using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.IO;
using TMPro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class ResourceConvert : MonoBehaviour
{
    public bool LOAD_NEW_RESOURCES;
    
    public List<string> pseudoName;
    
    public List<string> realName;
    public List<Sprite> imageList;
    public List<AudioClip> audioList;
    public List<string> audioListName;
    public TextAsset mainTA;
    public Core gameCore;
    // Start is called before the first frame update
    void Awake()
    {
        if (LOAD_NEW_RESOURCES)
        {

            mainTA = Resources.Load<TextAsset>("all_assets");
            string[] scriptLines = Regex.Split(mainTA.text, "\n|\r|\r\n");




            string line = "";
            int counter = 0;

            for (int i = 0; i < scriptLines.Length; i++)
            {
                line = scriptLines[i];
                var regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("image")));
                var match = regex.Match(line);
                if (match.Success && line.Contains("/"))
                {

                    pseudoName.Add(match.Groups[1].Value);
                    int istart = line.IndexOf("/") + "/".Length;
                    string posStr = line.Substring(istart, line.IndexOf(".") - istart);
                    realName.Add(posStr);

                }

                counter++;




            }

            for (int i = 0; i < realName.Count; i++)
                imageList.Add(Resources.Load<Sprite>("images/" + realName[i]));
          



            gameCore.SetMemory(pseudoName, realName);
        }
    }


}
