using UnityEngine;
using System.IO;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text.RegularExpressions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Core : MonoBehaviour
{
    public bool LOAD_NEW_SCENARIO;
    public string clearLine;
    public string lastSave;
    public GameObject AutoSave;
    public int maxLogSize;
    public float textDelay;
    public GameObject bgCanvas;
    private GameObject last3DBG;
    public float maxVolumeMusic;
    [HideInInspector] public bool Skipping;
    public float maxVolumeSound;
    public AudioSource MusicPlayer;
    public AudioSource SoundPlayer;
    public List<GameObject> BG3D;
    private bool[] BG3DSpawned_bool; 
    private GameObject[] BG3DSpawned; 
    public GameObject BG;
    public GameObject labelGroup;
    public GameObject spritePref;
    public GameObject defaultSpritePos;
    public GameObject Label;
    public GameObject ChooseBox;
    public LayoutGroup Group;
    public TextMeshProUGUI TextMP;
    public TextMeshProUGUI SayerMP;
    public Canvas downMenu;
    public Canvas mapCanvas;
    public Dictionary<string, Label> Labels;
    public int currLine = 0;
    public string currLabel ;
    public List<string> LabelName;
    public List<Label> LabelList;
    [HideInInspector] public bool Block;
    [HideInInspector] public int chooseReturnEdgeLine;
    [HideInInspector] public int chooseReturnPointLine;
    [HideInInspector] public int ifReturnEndLine;
    [HideInInspector] public int ifReturnStartLine;
    private int chooseNum = -1;
    [HideInInspector] public List<Choose> tmpChoose ;
    [HideInInspector] public List<SpriteRenderer> Sprites;
   
    public List<string> imagePseudoName;
    public List<string> imageRealName;
    public List<string> varName;
    public List<int> varValue;
    [HideInInspector] public List<string> logName;
    [HideInInspector] public List<string> logLine;
    [HideInInspector] public List<Color> logColor;
    
    [HideInInspector] public string currSaveNum;
    public TextAsset mainTA;
    [HideInInspector] public bool isTextCasting;
    
    
    [HideInInspector] public AudioSource toMax;
    [HideInInspector] public AudioSource toMin;
    [HideInInspector] public Image oldScene;
    [HideInInspector] public Image newScene;
    [HideInInspector] public int saveLineStart = 9999;
    [HideInInspector] public int saveLineFinish = 9999;
    [HideInInspector] public string lastAudio;
    [HideInInspector] public string saveSprites;
    [HideInInspector] public Label tmpLabelComp;

    public List<MapPoint> MapPoints;

    public void SetMemory(List<string> l1, List<string> l2)
    {
        imagePseudoName = l1;
        imageRealName = l2;
    }
    
    public void SetBlock(bool state)
    {
        Block = state;
        downMenu.enabled = !state;
    }


    public void LoadMediaCluster(List<string> labels)
    {
        foreach (var label  in labels)
        {
            LabelList[labelIndex(label)].PreloadImages();
            LabelList[labelIndex(label)].PreloadAudio();
        }
        
    }
    
    public void Save(string savenum)
    {
        PlayerPrefs.SetString("LastSave", savenum);
        lastSave = savenum;
        saveSprites = "";
        foreach (var sprite in Sprites)
        {
            if (sprite)
                saveSprites = saveSprites + sprite.name + "|" + sprite.transform.parent.position.x + "|" + sprite.transform.parent.position.z + "|";
            
        }

        if (Sprites.Count() > 0)
            PlayerPrefs.SetString(savenum + "Sprites", saveSprites);
        else 
            PlayerPrefs.SetString(savenum + "Sprites", "");
        if (newScene || last3DBG)
        {
            string sceneName;
            if (newScene)
                sceneName = newScene.sprite.name;
            else
                sceneName = last3DBG.name.Replace("(Clone)", "");
            
            int i = imageRealName.FindIndex(x => x.Equals(sceneName));
            PlayerPrefs.SetString(savenum + "Scene", imagePseudoName[i]);
            PlayerPrefs.SetString(savenum + "ScenePreview", imageRealName[i]);
        }
        else
            PlayerPrefs.SetString(savenum + "Scene", "");

        if (lastAudio.Length > 0)
            PlayerPrefs.SetString(savenum + "Audio", lastAudio);
        else
            PlayerPrefs.SetString(savenum + "Audio", "");
        PlayerPrefs.SetInt(savenum + "isSaved", 1);
        PlayerPrefs.SetInt(savenum + "Line", currLine);
        PlayerPrefs.SetString(savenum + "Label", currLabel);
        for (int i = 0; i < varName.Count; i++)
        {
            PlayerPrefs.SetInt(savenum + varName[i], varValue[i]);
        }
        PlayerPrefs.SetString(savenum + "Savetime", "" + System.DateTime.Now);
        PlayerPrefs.SetInt(savenum + "CREL", chooseReturnEdgeLine);
        PlayerPrefs.SetInt(savenum + "CRPL", chooseReturnPointLine);
        PlayerPrefs.SetInt(savenum + "IREL", ifReturnEndLine);
        PlayerPrefs.SetInt(savenum + "IRSL", ifReturnStartLine);
        PlayerPrefs.Save();
    }

    public void QuickSave()
    {
        Save("Quick");

    }
    
    public void Load(string savenum)
    {
        if (PlayerPrefs.GetInt(savenum + "isSaved") == 1)
        {
            chooseReturnEdgeLine = PlayerPrefs.GetInt(savenum + "CREL");
            chooseReturnPointLine = PlayerPrefs.GetInt(savenum + "CRPL");
            ifReturnEndLine = PlayerPrefs.GetInt(savenum + "IREL");
            ifReturnStartLine = PlayerPrefs.GetInt(savenum + "IRSL");
            currSaveNum = savenum;
            string tmpSave = PlayerPrefs.GetString(savenum + "Sprites");
            string sprite = "";
            string pos = "";
            string pos2 = "";
            int tmpLine = PlayerPrefs.GetInt(savenum + "Line");
            saveLineStart = tmpLine;
            saveLineFinish = saveLineStart;
        
            tmpLabelComp = LabelList[labelIndex(PlayerPrefs.GetString(savenum + "Label"))];
            while (tmpSave.Length > 2)
            {
                saveLineFinish++;
                sprite = tmpSave.Substring(0, tmpSave.IndexOf("|"));
                tmpSave = tmpSave.Remove(0, tmpSave.IndexOf("|") + 1);
                pos = tmpSave.Substring(0, tmpSave.IndexOf("|"));
                tmpSave = tmpSave.Remove(0, tmpSave.IndexOf("|") + 1);
                pos2 =tmpSave.Substring(0, tmpSave.IndexOf("|"));
                pos = pos.Replace(",", ".");
                pos2 = pos2.Replace(",", ".");
                tmpSave = tmpSave.Remove(0, tmpSave.IndexOf("|") + 1);
                tmpLabelComp.scenarioBlock.Insert(tmpLine, "    show " + sprite + " at custom (" + pos + ", " + pos2 + ")");
            }

            if (PlayerPrefs.GetString(savenum + "Audio") != ""){
                tmpLabelComp.scenarioBlock.Insert(tmpLine,
                    "    play music \"audio/" + PlayerPrefs.GetString(savenum + "Audio") + ".mp3");
                saveLineFinish++;
            }
            if (PlayerPrefs.GetString(savenum + "Scene") != "")
                tmpLabelComp.scenarioBlock.Insert(tmpLine, "    scene " + PlayerPrefs.GetString(savenum + "Scene"));
            else
                tmpLabelComp.scenarioBlock.Insert(tmpLine, "    scene black");
            saveLineFinish++;
            currLabel = PlayerPrefs.GetString(savenum + "Label");
            
            PreloadResources();
            
            currLine = saveLineStart;
            if (toMax)
                Destroy(toMax.gameObject);
            for (int i = 0; i < varName.Count; i++)
            {
                varValue[i] = PlayerPrefs.GetInt(savenum + varName[i]);
            }


            Step();
           
        }
    }

    public void QuickLoad()
    {
        Load("Quick");
    }





    
    
    public void FlushSprites()
    {
        foreach (var sprite in Sprites)
        {
            if (sprite)
                Destroy(sprite.gameObject);
        }
        Sprites.Clear();
    }

    DateTime ParseDate(string date)
    {
        return DateTime.ParseExact(date, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        
    }
    void Start()
    {
        if (LOAD_NEW_SCENARIO)
        {
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

            currLabel = "START";
            currLine = 0;
        }
        else
        {
            foreach (Transform label in labelGroup.transform)
            {
                LabelList.Add(label.GetComponent<Label>());
                LabelName.Add(label.GetComponent<Label>().name);
            }
        }

        BG3DSpawned = new GameObject[BG3D.Count];
        BG3DSpawned_bool = new bool [BG3D.Count];
        maxVolumeMusic = PlayerPrefs.GetFloat("MusicVolume");
        maxVolumeSound = PlayerPrefs.GetFloat("SoundVolume");
        textDelay = PlayerPrefs.GetFloat("TextDelay");
        currLabel = LabelList[0].labelName;
        
        PreloadResources();
        
        currLine = 0;
        lastSave = PlayerPrefs.GetString("LastSave");
        if (lastSave != String.Empty)
        {
            Load(lastSave);
        }
        else
        {
            Step();     
        }



    }

    public int labelIndex(string name)
    {
        
        for(int i=0; i<LabelName.Count; i++)
        {
            if (LabelName[i] == name)
                return i;
        }

        return 0;
    }
    
    

    void PlayMusic(AudioClip clip)
    {
        if (toMax)
            toMin = toMax;
        toMax = Instantiate(MusicPlayer, transform).GetComponent<AudioSource>();
        lastAudio = clip.name;
        toMax.clip = clip;
        toMax.Play();
        toMax.GetComponent<Fader>().maxVolumeMusic = maxVolumeMusic;
        if (toMin)
            toMin.GetComponent<Fader>().fadingOut = true;
        toMax.GetComponent<Fader>().fadingIn = true;
    }
    void PlaySound(AudioClip clip)
    {
        SoundPlayer.volume = maxVolumeSound;
        SoundPlayer.PlayOneShot(clip);

    }
    private void FixedUpdate()
    {
        if (Skipping)
        {
            Step();
        }

    }

    string GetCurLineFromCurLabel()
    {
        string line = LabelList[labelIndex(currLabel)].scenarioBlock[currLine];
        return line;
    }

    void SetCurLineInCurLabel(string line)
    {
        LabelList[labelIndex(currLabel)].scenarioBlock[currLine] = line;
    }
    void textCast()
    {
        string line = GetCurLineFromCurLabel();
        
        SayerMP.text = "";
        SayerMP.color = new Color(0.82f, 0.41f, 0.12f);
        
        if (line.Contains(" a ") || line.Contains("a_nar "))
        {
            SayerMP.color = new Color(0.16f, 0.97f, 0.93f);
            SayerMP.text = "Алина";
        }
        if (line.Contains("asa "))
        {
            SayerMP.color = new Color(0.96f, 0, 0);
            SayerMP.text = "☠ А̷̞̞̖͔̹̍͌л̵̨̛͔̳̝̏͝и̴̱̉͌̒н̴͓̲͇́̑͂̋̋а̸͎͕͓̽́̽ ⛧";
        }
        
        if (line.Contains("mr ") || line.Contains("mr_nar "))
        {
            SayerMP.color = new Color(0, 0.86f, 0.42f);
            SayerMP.text = "Мира";
        }
        if (line.Contains("mr1 "))
        {
            SayerMP.color = new Color(0, 0.86f, 0.42f);
            SayerMP.text = "Мира 1";
        }
        if (line.Contains("mr2 "))
        {
            SayerMP.color = new Color(0, 0.86f, 0.42f);
            SayerMP.text = "Мира 2";
        }
        if (line.Contains("mr3 "))
        {
            SayerMP.color = new Color(0, 0.86f, 0.42f);
            SayerMP.text = "Мира 3";
        }
        if (line.Contains("mr4 "))
        {
            SayerMP.color = new Color(0, 0.86f, 0.42f);
            SayerMP.text = "Мира 4";
        }
        if (line.Contains("ms ") || line.Contains("ms_nar "))
        {
            SayerMP.color = new Color(0.84f, 0.71f, 0.36f);
            SayerMP.text = "Миша";
        }
        if (line.Contains(" k ") || line.Contains("k_nar "))
        {
            SayerMP.color = new Color(0.77f, 0.24f, 1);
            SayerMP.text = "Кир";
        }
        if (line.Contains("mo ") || line.Contains("mo_nar "))
        {
            SayerMP.text = "Мама";
        }
        if (line.Contains(" n ") || line.Contains("n_nar "))
        {
            SayerMP.color = new Color(0.99f, 0.74f, 0.73f);
            SayerMP.text = "Наташа";
        }
        if (line.Contains(" s ") || line.Contains("so_nar "))
        {
            SayerMP.color = new Color(1, 0.86f, 0.35f);
            SayerMP.text = "Cоня";
        }
        if (line.Contains(" t ") || line.Contains("t_nar "))
        {
            SayerMP.text = "Толя";
        }
        if (line.Contains(" u "))
        {
            SayerMP.text = "Юля";
        }
        if (line.Contains(" p "))
        {
            SayerMP.text = "Профессор";
        }
        if (line.Contains("doc "))
        {
            SayerMP.text = "Доктор";
        }
        if (line.Contains("pat "))
        {
            SayerMP.text = "Пациент 1";
        }
        if (line.Contains("pat2 "))
        {
            SayerMP.text = "Пациент 2";
        }
        if (line.Contains("ohr "))
        {
            SayerMP.text = "Охранник";
        }
        if (line.Contains("un ") || line.Contains("un_nar "))
        {
            SayerMP.text = "???";
        }
        if (line.Contains("ke "))
        {
            SayerMP.text = "Кеша";
        }   
        if (line.Contains("nothing "))
        {
            SayerMP.text = "Пустота";
        } 
        if (line.Contains("pud "))
        {
            SayerMP.text = "Лужица";
        } 
        if (line.Contains("ag1 "))
        {
            SayerMP.text = "Агент 0";
        } 
        if (line.Contains("ag2 "))
        {
            SayerMP.text = "Агент 1";
        } 
        if (line.Contains("emp "))
        {
            SayerMP.text = "Провода, что постоянно жужжат";
        } 
        if (line.Contains("ub "))
        {
            SayerMP.text = "Уборщица";
        } 
        if (line.Contains("narrator "))
        {
            SayerMP.text = "";
        } 
        if (line.Contains("mr_chat "))
        {
            SayerMP.text = "From: Мира";
        } 
        if (line.Contains("s_chat "))
        {
            SayerMP.text = "From: Соня";
        } 
        if (line.Contains("k_chat "))
        {
            SayerMP.text = "From: Unknown";
        } 
        if (line.Contains("k2_chat "))
        {
            SayerMP.text = "From: Кир";
        }

        if (logName.Count >= maxLogSize)
        {
            logName.Remove(logName[0]);
            logColor.Remove(logColor[0]);
        }
        logColor.Add(SayerMP.color);
        logName.Add(SayerMP.text);
        string tmpOut;
        
        tmpOut = Regex.Replace(GetCurLineFromCurLabel(), "[A-z]|[0-9+]|\"|{|}|=|#", string.Empty);
        tmpOut = tmpOut.Trim();
        
        if (logLine.Count >= maxLogSize)
            logLine.Remove(logLine[0]);
        logLine.Add(tmpOut);
        if (!Skipping && textDelay>0)
            StartCoroutine(textCastEnum(tmpOut));
        else
        {
            TextMP.text = tmpOut;
            currLine++;
        }
    }

    private IEnumerator textCastEnum(string line)
    {
        clearLine = line;
        isTextCasting = true;
        for (int i = 0; i < line.Length+1; i++)
        {
            TextMP.text = line.Substring(0, i);
            yield return new WaitForSeconds(textDelay);
        }
        currLine++;
        isTextCasting = false;
    }

    string nextLabel(string name)
    {
        return LabelName[labelIndex(name) + 1];
    }

    void jumpLabel()
    {
        currLabel = GetCurLineFromCurLabel().Split(' ').Last();
        currLabel =   Regex.Replace(currLabel, @"[ \r\n\t]", "");
        
        PreloadResources();
        
        currLine = 0;
    }

    public void outerJumpLabel(string label)
    {
        chooseReturnEdgeLine = 9999;
        currLabel = label;
        currLine = 0;
        downMenu.enabled = true;
        mapCanvas.enabled = false;
        Step();
    }
    public void ChooseConseq(int line)
    {
        currLine = line;
    }

    public void ChooseReturn(int line)
    {
        chooseReturnEdgeLine = line-1;
    }
    void generateMenu()
    {
        tmpChoose.Clear();
        chooseNum = -1;
       
        currLine++;
        Block = true;
        while ((GetCurLineFromCurLabel() == "" || GetCurLineFromCurLabel().Substring(0, 7) ==
                   "       ") && currLine < LabelList[labelIndex(currLabel)].scenarioBlock.Count)
        {
                       
            if (GetCurLineFromCurLabel().Contains(":"))
            {
                            
                            
                chooseNum++;
                if (chooseNum-1>=0) 
                    tmpChoose[chooseNum-1].SetReturnPoint(currLine+1);
                Choose choose;
                choose = Instantiate(ChooseBox, Group.transform).GetComponent<Choose>();
                choose.SetLookup(currLine + 1);
                string tmpOut; 
                tmpOut = Regex.Replace(GetCurLineFromCurLabel(), "[A-z]|[0-9+]|\"|{|}|=|#|:", string.Empty);
                tmpOut = tmpOut.Trim();
                choose.SetText(tmpOut);
                choose.SetParent(gameObject);
                tmpChoose.Add(choose);
            }
            
                       
            currLine++;
                       
            chooseReturnPointLine = currLine;
                        

        }

        tmpChoose[chooseNum].Return = chooseReturnPointLine;
    }
    void generateIf()
    {
        currLine++;
        ifReturnStartLine = currLine;
        while ((GetCurLineFromCurLabel() == "" || GetCurLineFromCurLabel().Substring(0, 7) ==
                   "       ") && currLine < LabelList[labelIndex(currLabel)].scenarioBlock.Count)
        {
                       
                        
                       
            currLine++;
                       
           
                        

        }
        ifReturnEndLine = currLine;
    }

    void castSprite(SpriteRenderer sprite, string name)
    {
       try{
            
            int i = LabelList[labelIndex(currLabel)].illustrationName.FindIndex(x => x.Equals(name));
            sprite.sprite = LabelList[labelIndex(currLabel)].illustrationImage[i];
        }
         catch (Exception e)
         {
            Debug.Log("SpriteTrouble");
         }
      
    }

    public Sprite GetSpriteFromCluster(string name)
    {
        int i = LabelList[labelIndex(currLabel)].illustrationName.FindIndex(x => x.Equals(name));
        return  (LabelList[labelIndex(currLabel)].illustrationImage[i]);
    }


    void loadSceneCase(int num)
    {
        if(last3DBG)
            last3DBG.SetActive(false);
        if (BG3DSpawned_bool[num]){
            BG3DSpawned[num].SetActive(true);
            last3DBG = BG3DSpawned[num];
        }
        else
        {
            last3DBG = Instantiate(BG3D[num]);
            BG3DSpawned[num] = last3DBG;
            BG3DSpawned_bool[num] = true;
        }
    }

    void loadScene(string name)
    {
        if (newScene)
            newScene.GetComponent<BGObject>().Destructor();
        bool is_BG3D = false;
        FlushSprites();
        switch (name)
        {
            case "Construct":
            {
                is_BG3D = true;
                loadSceneCase(0);
               
            } break;
        }
        if (!is_BG3D)
            try
            {
                int i = LabelList[labelIndex(currLabel)].illustrationName.FindIndex(x => x.Equals(name));
               
              
                    oldScene = newScene;
                    newScene = Instantiate(BG, bgCanvas.transform).GetComponent<Image>();
                    newScene.sprite =  LabelList[labelIndex(currLabel)].illustrationImage[i];
                    newScene.transform.SetAsLastSibling();
                    if (oldScene)
                        oldScene.GetComponent<BGObject>().Destructor();


                
            }
             catch (Exception e)
             {
                 Debug.Log("Проблема с фоном " + name);
             }
        
    }
    
    void loadChar(string name, string type, float pos, float pos2 =0)
    {
        
        SpriteRenderer sprite = null;
        var position = defaultSpritePos.transform.position;
        if (type == "Normal")
        {
            if (pos < 0.25f)
                pos = 0.25f;
            if (pos > 0.75f)
                pos = 0.75f;
            sprite = Instantiate(spritePref, new Vector3((position.x + pos * 4.45f),0, position.z), Quaternion.identity, defaultSpritePos.transform).GetComponentInChildren<SpriteRenderer>();
        
        }

        if (type == "Zoomed")
        {
            if (pos < 0.25f)
                pos = 0.25f;
            if (pos > 0.75f)
                pos = 0.75f;
            sprite = Instantiate(spritePref, new Vector3((position.x + pos * 4.45f),-0.234f, position.z-0.581f), Quaternion.identity, defaultSpritePos.transform).GetComponentInChildren<SpriteRenderer>();
 
            
        }
        if (type == "Custom")
        {
            sprite = Instantiate(spritePref).GetComponentInChildren<SpriteRenderer>();
            Debug.Log("pos "+ pos2 );
            sprite.transform.parent.transform.position = new Vector3(pos, 0, pos2);

        }

        if (type == "Default")
        {
            sprite = Instantiate(spritePref, new Vector3((position.x + pos * 4.45f),0, position.z), Quaternion.identity, defaultSpritePos.transform).GetComponentInChildren<SpriteRenderer>();
   
        }

        castSprite(sprite, name);
        sprite.name = name;
        sprite.transform.parent.name = name;
        Sprites.Add(sprite);

    }

    void valueComparator(string line, string sign)
    {
    
            Debug.Log("Line contains " + sign);
            int compValue = 0;
            switch (sign)
            {
                case "<=":  compValue = -999;
                    break;
                case "!=":  compValue = 0;
                    break;
                case "==":  compValue = 999;
                    break;
                case ">=":  compValue = 999;
                    break;
                case ">":  compValue = 999;
                    break;
                case "<":  compValue = -999;
                    break;
            }
           
            var regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape(sign)));
            var match = regex.Match(line);
            if (match.Success)
            {
                compValue = int.Parse(match.Groups[1].Value);
            }
            regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("if")));
            match = regex.Match(line);
            if (match.Success)
            {
                int i = varName.FindIndex(x => x.Equals(match.Groups[1].Value));
                bool isTrue = false;
                switch (sign)
                {
                    case "<=":
                    {
                        isTrue = (varValue[i] <= compValue);
                    }
                        break;
                    case "!=":
                    {
                        isTrue = (varValue[i] != compValue);
                    }
                        break;
                    case "==":
                    {
                        isTrue = (varValue[i] == compValue);
                    }
                        break;
                    case ">=":
                    {
                        isTrue = (varValue[i] >= compValue);
                    }
                        break;
                    case "<":
                    {
                        isTrue = (varValue[i] < compValue);
                    }
                        break;
                    case ">":
                    {
                        isTrue = (varValue[i] > compValue);
                    }
                        break;
                        
                }
                if (isTrue)
                {
                    currLine = ifReturnStartLine;
                }
                else
                {
                    currLine = ifReturnEndLine;
                }
            }
                       
        
        
    }

    void renewVar()
    {
        var regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("$")));
        var match = regex.Match(GetCurLineFromCurLabel());
        if (match.Success)
        {
            int i = varName.FindIndex(x => x.Equals(match.Groups[1].Value));

            if (GetCurLineFromCurLabel().Contains("+="))
                varValue[i] += 1;
            if (GetCurLineFromCurLabel().Contains(" = ")){
                regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("=")));
                match = regex.Match(GetCurLineFromCurLabel());
                varValue[i] = int.Parse(match.Groups[1].Value);
            }
                      
        }
        currLine++;
        Step();
        return;
    }
    void setPause()
    {
        try
        {
            StartCoroutine(setPauseCoroutine(int.Parse(Regex.Match(GetCurLineFromCurLabel(), @"\d+").Value)));
        }
        catch (Exception e)
        {
            Debug.Log("pause error");
            currLine++;
            Step();
        }
    }

    IEnumerator setPauseCoroutine(int time)
    {
        SetBlock(true);
        yield return new WaitForSeconds(time);
        SetBlock(false);
        currLine++;
        Step();
    }
    void hideSprite(GameObject sprite)
    {
        if (!Skipping)
            sprite.GetComponent<SpriteObject>().Destructor();
        else 
            Destroy(sprite);
        for (int i = 0; i < Sprites.Count; i++)
        {
            if (Sprites[i] == null)
            {
                Sprites.RemoveAt(i);
            }
        }
        return;
    }

    void openMap(string currLineValue)
    {
        string tmpLine = currLineValue;
        List<string> clusterLabels = new List<string>();
        Debug.Log("КАРТА " + currLineValue);
        tmpLine = tmpLine.Remove(0, currLineValue.IndexOf("(")+1);
        tmpLine = tmpLine.Substring(0, tmpLine.Length-1);
        while (tmpLine.Contains(','))
        {
            clusterLabels.Add((tmpLine.Substring(0, tmpLine.IndexOf(",")).Trim(' ')));
            tmpLine = tmpLine.Remove(0, tmpLine.IndexOf(",") + 1);
        }
        clusterLabels.Add((tmpLine).Trim(' '));
        string mapPointName =  clusterLabels[(clusterLabels.Count-1)];
        clusterLabels.RemoveAt(clusterLabels.Count-1);
        string jumpName =  clusterLabels[(clusterLabels.Count-1)];
        clusterLabels.RemoveAt(clusterLabels.Count-1);
        
        switch (mapPointName)
        {
            case "ParkPoint":
                addMainQuest(0, clusterLabels, jumpName); 
                break;
        }
        
        downMenu.enabled = false;
        mapCanvas.enabled = true;

    }

    void addMainQuest(int num, List<string> Resources, string jumpName)
    {
        MapPoints[num].AddMainQuest(jumpName, Resources);
    }

    public void Skip(bool state)
    {
        Skipping = state;
        if (state == false)
        {
            isTextCasting = false;
        }
    }

    void PreloadResources()
    {
        if (LabelList[labelIndex(currLabel)].audioClips.Count < LabelList[labelIndex(currLabel)].audiosName.Count)
        {
            LabelList[labelIndex(currLabel)].PreloadImages();
            LabelList[labelIndex(currLabel)].PreloadAudio();
        }
    }
    
    public void Step()
    {
    
       
        if (currLine == saveLineFinish)
        {
  
            for (int i = 0; i < saveLineFinish-saveLineStart; i++)
            {
                GameObject.Find(PlayerPrefs.GetString( currSaveNum +  "Label")).GetComponent<Label>().scenarioBlock.RemoveAt(saveLineStart);
            }

            currLine = saveLineStart;
       
            saveLineFinish = 9999;
            saveLineStart = 9999;
        }


        if (!Block && !isTextCasting || !Block && Skipping)
        {
            StopAllCoroutines();
            if (chooseReturnEdgeLine + 1 == chooseReturnPointLine)
                chooseReturnEdgeLine += 1;
            if (currLine == chooseReturnEdgeLine)
            {
                currLine = chooseReturnPointLine;
                chooseReturnEdgeLine = 9999;
            }

            string currLineValue = "";
            try
            {
                currLineValue = GetCurLineFromCurLabel();
            }
            catch (Exception e)
            {
                Debug.Log("No string");
            }

            Debug.Log(currLineValue);
            

            if (currLine < LabelList[labelIndex(currLabel)].scenarioBlock.Count)
            {

                if (currLineValue.Contains("switchzone") && currLineValue.Contains("$")) //Если переход на след зону
                {

                    openMap(currLineValue);
                    return;
                }
               
                var regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("scene")));
                var match = regex.Match(currLineValue);
                if (match.Success)
                {
                    Debug.Log(match.Groups[1].Value);
                   loadScene(match.Groups[1].Value);
                   currLine++;
                   Step();
                   return;
                }
                
                regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("show")));
                match = regex.Match(currLineValue);
                if (match.Success)
                {
                    Debug.Log("SPRITE APPEARS");
                    bool isZoomed;
                    bool isNormal;
                    isZoomed = currLineValue.Contains("zoomer");
                    isNormal = currLineValue.Contains("normal");
                    CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                    ci.NumberFormat.CurrencyDecimalSeparator = ".";
                    float pos;
                    if (isNormal || isZoomed)
                    {
                       
                        int istart = currLineValue.IndexOf("(") + "(".Length;
                        string posStr = currLineValue.Substring(istart, currLineValue.IndexOf(")") - istart);
                       
                        
                        pos = float.Parse(posStr, NumberStyles.Any, ci);
                    
                        if (isNormal)
                            loadChar(match.Groups[1].Value, "Normal", pos);
                        if (isZoomed)
                            loadChar(match.Groups[1].Value, "Zoomed", pos);
                        currLine++;
                        Step();
                        return;
                    }

                    if (currLineValue.Contains("custom"))
                    {
                        Debug.Log("CUSTOM");
                        int istart = currLineValue.IndexOf("(", StringComparison.Ordinal) + "(".Length;
                        string posStr = currLineValue.Substring(istart, currLineValue.IndexOf(")", StringComparison.Ordinal) - istart);
                        string first = posStr.Substring(0, posStr.IndexOf(",", StringComparison.Ordinal));
                        string second = posStr.Substring(posStr.IndexOf(",", StringComparison.Ordinal)+1, posStr.Length-1 - posStr.IndexOf(",", StringComparison.Ordinal));
                        Debug.Log(first + " " + second);
                        
                        pos = float.Parse(first, NumberStyles.Any, ci);
                        float pos2 = float.Parse(second, NumberStyles.Any, ci);
                        loadChar(match.Groups[1].Value, "Custom", pos, pos2);
                        currLine++;
                        Step();
                        return;
                    }
                    loadChar(match.Groups[1].Value, "Default", 0.5f);
                    currLine++;
                    Step();
                    return;
                
                   
                    //Debug.Log(match.Groups[1].Value);
                    
          
                }
                
                regex = new Regex(string.Format(@"(?<!\w){0}\W+(\w+)", Regex.Escape("hide")));
                match = regex.Match(currLineValue);
                if (match.Success)
                {
                    GameObject toDelete;
                    if (toDelete = GameObject.Find(match.Groups[1].Value)) 
                    {
                        Debug.Log("Sprite found");
                        hideSprite(toDelete);
                    }

                    currLine++;
                    Step();
                    return;
                }

                if (currLineValue.Contains("play music"))
                {
                    
                    int istart = currLineValue.IndexOf("/") + "/".Length;
                    string posStr = currLineValue.Substring(istart, currLineValue.IndexOf(".") - istart);
                
                 
                    int i = LabelList[labelIndex(currLabel)].audiosName.FindIndex(x => x.Equals(posStr));
                 
                    AudioClip clip = LabelList[labelIndex(currLabel)].audioClips[i];
                 
                    PlayMusic(clip);
                }
                if (currLineValue.Contains("play sound"))
                {
                    
                    int istart = currLineValue.IndexOf("/") + "/".Length;
                    string posStr = currLineValue.Substring(istart, currLineValue.IndexOf(".") - istart);
                    
                    int i = LabelList[labelIndex(currLabel)].audiosName.FindIndex(x => x.Equals(posStr));
                    
                    //int i = audioNameList.FindIndex(x => x.Equals(posStr));
                   // AudioClip clip = audioList[i];
                    AudioClip clip = LabelList[labelIndex(currLabel)].audioClips[i];
                    PlaySound(clip);
                }

                
                if (currLineValue.Contains("$")  && ! currLineValue.Contains("renpy") && !currLineValue.Contains("quick_menu") && !currLineValue.Contains("save_name" )) //Если обновление переменной
                {
                    renewVar();
                    return;
                }
                
                if (currLineValue.Contains("renpy.pause")) //Если пауза
                {
                   
                    setPause();
                    return;
                }

                if (currLineValue.Contains("if"))
                {
                    
                    Debug.Log("УСЛОВИЕ");
                    generateIf();
                    string nextStr = LabelList[labelIndex(currLabel)].scenarioBlock[ifReturnStartLine - 1];
                    if (nextStr.Contains("<="))
                       valueComparator(nextStr, "<=");
                    if (nextStr.Contains(">="))
                        valueComparator(nextStr, ">=");
                    if (nextStr.Contains(">"))
                        valueComparator(nextStr, ">");
                    if (nextStr.Contains("<"))
                        valueComparator(nextStr, "<");
                    if (nextStr.Contains("!="))
                        valueComparator(nextStr, "!=");
                    if (nextStr.Contains("=="))
                        valueComparator(nextStr, "==");
                    Step();
                    return;
                }

                if (currLineValue.Contains("menu:")) //Если меню
                {
                    AutoSave.GetComponent<LoadButton>().SaveOverride();
                    Debug.Log("ВЫБОР");
                    SetBlock(true);
                    
                    generateMenu();
                    return;
                    
                }
                
                
                if (currLineValue.Contains("jump")) //Если прыжок
                {

                    Debug.Log("ПРЫЖОК " + currLineValue);
                    chooseReturnEdgeLine = 9999;
                    jumpLabel(); 
                    Step();
                    Debug.Log(labelIndex(currLabel));
                    return;
                }
                
                

                if (new Regex("[А-я]").IsMatch(currLineValue)) //Если русский текст
                {
                    textCast();
                }
                else
                {
                    currLine++;
                    Step();
                }
                
            }
            else
            {
                currLabel = nextLabel(currLabel);
                PreloadResources();
                currLine = 0;
            }
        }


    }

}
 