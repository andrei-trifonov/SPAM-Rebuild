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
    public bool LOAD_NEW_SCENARIO;
    public List<string> pseudoName;
    public List<string> realName;
    public List<Sprite> imageList;
    public GameObject labelGroup;
    public TextAsset mainTA;
    public TextAsset resourcesTA;
    public Core gameCore;
    
    public GameObject Label;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (LOAD_NEW_RESOURCES)
        {

            
            string[] scriptLines = Regex.Split(resourcesTA.text, "\n|\r|\r\n");




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
        if (LOAD_NEW_SCENARIO)
        {
            Label newLabel1;
            newLabel1 = Instantiate(Label, transform.position, transform.rotation).GetComponent<Label>();
            newLabel1.labelName = "START";
            gameCore.LabelName.Add(newLabel1.labelName);
            gameCore.LabelList.Add(newLabel1);
            gameCore.currLabel = "START";

            
            string[] scriptLines = Regex.Split(mainTA.text, "\n");




            string line = "";
            int counter = 0;

            for (int i = 0; i < scriptLines.Length; i++)
            {
                line = scriptLines[i];
                counter++;
                if (line.Contains("$ ") && !line.Contains("renpy") && !line.Contains("quick_menu") &&
                    !line.Contains("save_name"))
                {
                    // Debug.Log("Нашел переменную");



                    var regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("$")));
                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        if (gameCore.varName.Find(x => x.Equals(match.Groups[1].Value)) == null)
                        {
                            gameCore.varName.Add(match.Groups[1].Value);
                            regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("=")));
                            match = regex.Match(line);
                            if (match.Success)
                            {

                                if (match.Groups[1].Value == "True")
                                    gameCore.varValue.Add(1);
                                if (match.Groups[1].Value == "False")
                                    gameCore.varValue.Add(0);
                                else
                                {
                                    try
                                    {
                                        gameCore.varValue.Add(int.Parse(match.Groups[1].Value));
                                    }
                                    catch (Exception e)
                                    {
                                        gameCore.varName.RemoveAt(gameCore.varName.IndexOf(gameCore.varName.Last()));
                                    }

                                }

                            }

                        }

                    }
                }

                if (line.Contains("label "))
                {
                    // Debug.Log("Нашел главу");

                    
                    Label newLabel;
                    newLabel = Instantiate(Label, transform.position, transform.rotation, labelGroup.transform)
                        .GetComponent<Label>();
                   
                    string formattedLableName = line.Split(' ').Last().Substring(0, line.Split(' ').Last().Length - 1);
                    formattedLableName =   Regex.Replace(formattedLableName, @"[ \r\n\t]", "");
                    formattedLableName = formattedLableName.Substring(0, formattedLableName.Length - 1);
                    newLabel.labelName = formattedLableName;
                    newLabel.gameObject.name = formattedLableName;
                    gameCore.LabelName.Add (formattedLableName);
                    gameCore.LabelList.Add(newLabel);
                    gameCore.currLabel = formattedLableName;
                    continue;
                }

              
                foreach (var ImageName in gameCore.imagePseudoName)
                {
                    if (line.Contains(" " + ImageName + " "))
                    {
                        if (gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].illustrationName.FindIndex(x => x.Equals(ImageName))<0)
                        {
                            gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].illustrationName.Add(ImageName);
                            gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].illustrationRName
                                .Add(gameCore.imageRealName[gameCore.imagePseudoName.IndexOf(ImageName)]);
                        }

                        break;
                    }
                    
                        
                }
                
                if (line.Contains("play music") || line.Contains("play sound"))
                {
                
                    int istart = line.IndexOf("/") + "/".Length;
                    string posStr = line.Substring(istart, line.IndexOf(".") - istart);
                  
                    if (gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].audiosName.FindIndex(x => x.Equals(posStr)) < 0)
                    {
                        gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].audiosName.Add(posStr);
                    }
                }

                    
                        
                
                if (Regex.IsMatch(line, "\\w"))  
                    gameCore.LabelList[gameCore.labelIndex(gameCore.currLabel)].scenarioBlock.Add(line);
                

            }

            gameCore.currLabel = "START";
            gameCore.currLine = 0;
        }
    }


}
