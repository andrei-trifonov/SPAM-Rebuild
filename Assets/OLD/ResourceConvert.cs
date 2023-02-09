using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using System.Text.RegularExpressions;

using UnityEngine.UI;




public class ResourceConvert : MonoBehaviour
{
    public bool LOAD_NEW_RESOURCES;
    public bool LOAD_NEW_SCENARIO;
    
    
    public List<Sprite> imageList;
    [SerializeField] private GameObject labelGroup;
    [SerializeField] private TextAsset mainTA;
    [SerializeField] private TextAsset resourcesTA;
    
    public List<string> LabelName;
    [SerializeField] private int currLine = 0;
    [SerializeField] private string currLabel ;
    public List<Label> LabelList;
    public List<string> varName;
    public List<int> varValue;
    public List<string> imagePseudoName;
    public List<string> imageRealName;
    [SerializeField] private Core gameCore;
    [SerializeField] private GameObject Label;
    
    
    public int labelIndex(string name)
    {
        
        for(int i=0; i<LabelName.Count; i++)
        {
            if (LabelName[i] == name)
                return i;
        }

        return 0;
    }
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

                    imagePseudoName.Add(match.Groups[1].Value);
                    int istart = line.IndexOf("/") + "/".Length;
                    string posStr = line.Substring(istart, line.IndexOf(".") - istart);
                    imageRealName.Add(posStr);

                }

                counter++;




            }

            for (int i = 0; i < imageRealName.Count; i++)
                imageList.Add(Resources.Load<Sprite>("images/" + imageRealName[i]));




            gameCore.SetMemory(imagePseudoName, imageRealName);






        }
       
    }

    private void Start()
    {
         if (LOAD_NEW_SCENARIO)
         {
             imagePseudoName = gameCore.GetImPseudoName();
             imageRealName = gameCore.GetImRealName();
            Label newLabel1;
            newLabel1 = Instantiate(Label, transform.position, transform.rotation).GetComponent<Label>();
            newLabel1.labelName = "START";
            LabelName.Add(newLabel1.labelName);
            LabelList.Add(newLabel1);
            currLabel = "START";

            
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
                        if (varName.Find(x => x.Equals(match.Groups[1].Value)) == null)
                        {
                            varName.Add(match.Groups[1].Value);
                            regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("=")));
                            match = regex.Match(line);
                            if (match.Success)
                            {

                                if (match.Groups[1].Value == "True")
                                    varValue.Add(1);
                                if (match.Groups[1].Value == "False")
                                    varValue.Add(0);
                                else
                                {
                                    try
                                    {
                                        varValue.Add(int.Parse(match.Groups[1].Value));
                                    }
                                    catch (Exception e)
                                    {
                                        varName.RemoveAt(varName.IndexOf(varName.Last()));
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
                    LabelName.Add (formattedLableName);
                    LabelList.Add(newLabel);
                    currLabel = formattedLableName;
                    continue;
                }

              
                foreach (var ImageName in imagePseudoName)
                {
                    if (line.Contains(" " + ImageName + " "))
                    {
                        if (LabelList[labelIndex(currLabel)].illustrationName.FindIndex(x => x.Equals(ImageName))<0)
                        {
                            LabelList[labelIndex(currLabel)].illustrationName.Add(ImageName);
                            LabelList[labelIndex(currLabel)].illustrationRName
                                .Add(imageRealName[imagePseudoName.IndexOf(ImageName)]);
                        }

                        break;
                    }
                    
                        
                }
                
                if (line.Contains("play music") || line.Contains("play sound"))
                {
                
                    int istart = line.IndexOf("/") + "/".Length;
                    string posStr = line.Substring(istart, line.IndexOf(".") - istart);
                  
                    if (LabelList[labelIndex(currLabel)].audiosName.FindIndex(x => x.Equals(posStr)) < 0)
                    {
                        LabelList[labelIndex(currLabel)].audiosName.Add(posStr);
                    }
                }

                    
                        
                
                if (Regex.IsMatch(line, "\\w"))  
                    LabelList[labelIndex(currLabel)].scenarioBlock.Add(line);
                

            }

            gameCore.SetLabelsData(
                LabelName,
                LabelList,
                varName,
                varValue,
                imageRealName,
                imagePseudoName);
        }        
    }
}
