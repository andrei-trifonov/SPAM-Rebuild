using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Random = UnityEngine.Random;

//TODO проверить загрузку музыки и переменных + драгсов+ подключить превьюху одну.
[System.Serializable]
public class SaveObject 
{
    public SaveObject()
    {
        unlockedInv = new List<UnlockMessage>();
        Variables = new List<VarItem>();
        Actors = new List<Actor>();
       
    }
    
    public GDB.TextDisplay textDisplay = GDB.TextDisplay.Dialogue;
    public int drugsCount =0 ;
    public List<UnlockMessage> unlockedInv;
    public GDB.BGName lastBG = GDB.BGName.None;
    public GDB.Music lastMusic = GDB.Music.None;
    public string lastCG = "";
    public GDB.Name lastAuthor = GDB.Name.Никто;
    public string lastLine = "";
    public List<VarItem> Variables;
    public string Savetime = "";
    public List<Actor> Actors ;
    public int lastLineNum = 0;
    public int lastLabelNum = 0;
    public string previewName = "";
}

[System.Serializable]

public class VarItem
{
    public VarItem(GDB.Variables varName, int varValue)
    {
        this.varName = varName;
        this.varValue = varValue;
    }
    public GDB.Variables varName;
    public int varValue;

}

[System.Serializable]
public class Actor
{
    public Actor(Vector3 Position, GDB.Name Name, GDB.Pose Pose)
    {
        this.Name = Name;
        this.Pose = Pose;
        this.Position = Position;
    }

    public GameObject obj;
    public Vector3 Position;
    public GDB.Name Name;
    public GDB.Pose Pose;
}

public class NewGameCore : MonoBehaviour
{
    [SerializeField] private Canvas textCanvas;
    private Animator textCanvasAnimator;
    [SerializeField] private GameObject investigationLabel;
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private TextMeshProUGUI textContent;
    [SerializeField] private TextMeshProUGUI textFS;
    [SerializeField] private GameObject FSPanel;
    [SerializeField] private Animator Camera;
    
    private Dialogue scenarioComposer;
    
    [SerializeField] private List<Dialogue> extensionScenarios_RU;
    [SerializeField] private List<Dialogue> extensionScenarios_EN;
    private List<Dialogue> extensionScenarios;
    private List<LabelSample> scenario;
    
    private float maxVolumeMusic = 1; //TODO
    private Coroutine c;
    private Coroutine sc;
    private float textDelay = 0.1f; //TODO 
    private bool skipping; 
    
    private int labelNum;
    private int lineNum;
 
    private string castingLine;
    private bool loadingBG;
    private Coroutine lBGcoroutine;
    private bool loadingCG;
    private Coroutine lCGcoroutine;
    private bool loadingMusic;
    private Coroutine lMcoroutine;
    private bool loadingSound;
    private Coroutine lScoroutine;
    private Coroutine BSLMCoroutine; 
    private Coroutine menuCoroutine; 
    [SerializeField] private LogComposer Log;
    
    private List<GameObject> choiseBoxes = new List<GameObject>();
    [SerializeField] private GameObject choiseBox;
    [SerializeField] private LayoutGroup choiseGroup;
    [SerializeField] private GameObject menuTimerSlider;
    private bool choiseRoulette;

    [SerializeField] private InvestigationController invController;
    private bool isGameRunning = false;
    [SerializeField] private SpriteRenderer CG;
    [SerializeField] private VideoPlayer BG;
    private VideoClip clipBG;

    private AudioSource fadeInMusic;
    private AudioSource fadeOutMusic;
    [SerializeField] GameObject musicPlayer;
    [SerializeField] private Animator musicIcon;
    [SerializeField] private TMP_Text musicName;
    [SerializeField] private TMP_Text musicAuthor;
    private List<Track> TrackList;
    [SerializeField] private AudioSource sceneAudioPlayer;
    [SerializeField] private AudioSource soundPlayer;

    [SerializeField] private GameObject saveLoadMarker;

    private List<Actor> actorsOnScene = new List<Actor>();
    
    private bool loading;
    private bool hideText;
    private bool menu;
    
    public List<int> CoroutinesWorking;
    int cid;
    public SaveObject saveObj = new SaveObject();
    private string saveJString = "";

    private int curLocalization;
    private GameObject Emoji;
    
    [SerializeField] private List<GameObject> emojiList = new List<GameObject>();
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private Transform EventCanvas;

    private bool QTE;
    public void SetQTE(bool state)
    {
        if (!textCanvasAnimator)
        {
            textCanvasAnimator = textCanvas.GetComponent<Animator>();
        }

        QTE = state;
        EnableText(!state);
    }

    public int GetLocalization()
    {
        return curLocalization;
    }
    public void ChangeLocalization(int num)
    {
        if (!loading && CoroutinesWorking.Count == 0)
        {
            if (num == 0)
            {
                extensionScenarios = extensionScenarios_RU;
            }
            else
            {
                extensionScenarios = extensionScenarios_EN;
            }

            curLocalization = num;
            scenarioComposer = extensionScenarios[0];
            for (int i = 1; i < extensionScenarios.Count; i++)
            {
                foreach (var label in extensionScenarios[i].Labels)
                {
                    scenarioComposer.Labels.Add(label);

                }
            }
        }
    }

    public void SetTextMarker(bool state)
    {   
        textCanvasAnimator.SetBool("Ready", !state);
    }
    public void EnableText(bool state)
    {
        
        hideText = !state;
        if (QTE)
	     hideText = true;
	textCanvasAnimator.SetBool("Hide", hideText);
        
    }
    public void SetTextDelay(float speed)
    {
        textDelay = speed;
    }
    public void SetMusicSettings(float value)
    {
        maxVolumeMusic = value;
        if (fadeInMusic)
        {
            fadeInMusic.volume = value;
            fadeInMusic.GetComponent<Fader>().maxVolumeMusic = value;
        }
    }
    public void SetSoundSettings(float value)
    {
        soundPlayer.volume = value;
    }
    public void SetSceneAudioSettings(float value)
    {
        sceneAudioPlayer.volume = value;
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("Localization") == "")
        {
            if (Application.systemLanguage == SystemLanguage.Russian ||
                Application.systemLanguage == SystemLanguage.Ukrainian ||
                Application.systemLanguage == SystemLanguage.Belarusian)
            {
                    curLocalization = 0;
                    PlayerPrefs.SetString("Localization", "Russian");
                    Debug.Log("Installed RUS locale");
                    extensionScenarios = extensionScenarios_RU;
                }
            else
            {
                    curLocalization = 1;
                    PlayerPrefs.SetString("Localization", "English");
                    Debug.Log("Installed ENG locale");
                    extensionScenarios = extensionScenarios_EN;
                }

        }
      
        if (PlayerPrefs.GetString("Localization") == "Russian")
        {
            curLocalization = 0;
            extensionScenarios = extensionScenarios_RU;
        }
        else
        {
            curLocalization = 1;
            extensionScenarios = extensionScenarios_EN;
        }


        

        TrackList = GetComponent<OSTList>().GetTrackList();
        saveObj = new SaveObject();
        textCanvasAnimator = textCanvas.GetComponent<Animator>();
        scenarioComposer = extensionScenarios[0];
        for (int i = 1; i < extensionScenarios.Count; i++)
        {
            foreach (var label in extensionScenarios[i].Labels)
            {
                scenarioComposer.Labels.Add(label);

            } 
        }
        scenario = scenarioComposer.Labels;
        string savename = PlayerPrefs.GetString("LastSave");
        
        SetMusicSettings(PlayerPrefs.GetFloat("MusicVolume"));
   
        SetSoundSettings(PlayerPrefs.GetFloat("SoundVolume"));
    
        SetSceneAudioSettings(PlayerPrefs.GetFloat("SceneVolume"));
  
        SetTextDelay(PlayerPrefs.GetFloat("TextDelay"));
    
        if (savename.Length>0){
            Debug.Log(savename);
            Load(savename);
            }
        else
            Step();
    }
    public void ClearSprites()
    {
        foreach (Actor obj in actorsOnScene)
        {
            try { Destroy(obj.obj); }
            catch { }
        }
        actorsOnScene.Clear();
    }

    void BlinkSLMarker(int state)
    {
        try { StopCoroutine(BSLMCoroutine); } catch { }
        BSLMCoroutine = StartCoroutine(BlinkSLMarkerCoroutine(state));
    }

    IEnumerator BlinkSLMarkerCoroutine(int state)
    {
        saveLoadMarker.SetActive(true);
        foreach (Transform label in saveLoadMarker.transform)
        {
            label.gameObject.SetActive(false);
        }
        saveLoadMarker.transform.GetChild(state).gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        saveLoadMarker.SetActive(false);
        saveLoadMarker.transform.GetChild(0).gameObject.SetActive(false);
        saveLoadMarker.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void Load(string savenum)
    {
        
        if (!loading && CoroutinesWorking.Count == 0 )
        {
            //Clean scene
            
            menuTimerSlider.SetActive(false);
            StopAllCoroutines();
            loading = true;
            skipping = false;

            ClearSprites();
            CG.enabled = false;
            ClearBG();
                
            
            ApplyTextEffects(GDB.Fonts.Regular);
            textContent.text = "";
            textFS.text = "";
            textAuthor.text = "";
                
            menuTimerSlider.SetActive(false);
            SaveObject loadedData = JsonUtility.FromJson<SaveObject>(PlayerPrefs.GetString(savenum+"Save"));
         
            saveObj = loadedData;
            if (saveObj == null)
            {
                saveObj = new SaveObject();
                loading = false;
                return;
            }
            
            //Load music
            Item item = new Item();
            item.music = loadedData.lastMusic;
    
            item.show = true;

            
            if (loadedData.lastMusic!= GDB.Music.None)
                musicAction(item, true);
            if (loadedData.lastCG == "")
            {

                //Get actors
                foreach (var actor in loadedData.Actors)
                {
                    item = new Item();
                    item.pose = actor.Pose;
                    item.name = actor.Name;
                    item.V3position = actor.Position;

                    actorAction(item, true);
                }


                //Spawn Scene
                item = new Item();
                item.BGname = loadedData.lastBG;
                if (loadedData.lastBG != GDB.BGName.None)
                    bgAction(item, true);
                item = new Item();
                item.CGname = loadedData.lastCG;
            }

            else 
            {
            Debug.Log(loadedData.lastCG);
                item = new Item();
                item.show = true;
                item.CGname = loadedData.lastCG;
                cgAction(item, true);
            }
            //Return line num label num
            lineNum = loadedData.lastLineNum;
            labelNum = loadedData.lastLabelNum;

            
            item = new Item();
            item.line = loadedData.lastLine;
            item.name = loadedData.lastAuthor;
            if (loadedData.textDisplay == GDB.TextDisplay.Dialogue)
            {
                //return text to panel
                textAction(item, true);
            }
            else if (loadedData.textDisplay == GDB.TextDisplay.Fullscreen)
            {
                FSTextAction(item, true);
            }
            else if (loadedData.textDisplay == GDB.TextDisplay.Chat)
            {
                chatAction(item, true);
            }
            
         GetComponent<CameraMoveZoom>().NewMove(new Vector3 (0.14f, 1.86f,  45f),45.45f );

          // ApplyEffects( GDB.Effects.PointToAndZoom, false, 45.45f, );
           
            
            BlinkSLMarker(1);
            loading = false;
           
          
        }
    }
    public void Save(string savenum)
    {
        if (!QTE && !loading && CoroutinesWorking.Count == 0)
        {

            List<Actor> actors = new List<Actor>();
            foreach (Actor actor in actorsOnScene)
            {
                actors.Add(new Actor(actor.obj.transform.position, actor.Name, actor.Pose));
            }

            saveObj.Savetime = "" + System.DateTime.Now;
            saveObj.Actors = actors;
            saveObj.lastLineNum = lineNum;
            saveObj.lastLabelNum = labelNum;
            if (saveObj.lastCG!="")
                saveObj.previewName = saveObj.lastCG;
            else if (saveObj.lastBG != GDB.BGName.None)
                saveObj.previewName = saveObj.lastBG + "Prev";
            else 
                saveObj.previewName = "Black";
            saveJString = JsonUtility.ToJson(saveObj);
            Debug.Log("JSON " + saveJString );
            Debug.Log("Actors " );
            foreach (var actor in saveObj.Actors)
            {
                Debug.Log( actor.Name); 
                Debug.Log( actor.Position);
            }
          
            Debug.Log("AuthorText "  + saveObj.lastAuthor);
            Debug.Log("LineText "  + saveObj.lastLine);
            Debug.Log("Scene "  + saveObj.lastBG);
            Debug.Log("CG "  + saveObj.lastCG);
            Debug.Log("Music "  + saveObj.lastMusic);
            Debug.Log("Line " + saveObj.lastLineNum);
            Debug.Log("Label "  + saveObj.lastLabelNum);

            PlayerPrefs.SetString("LastSave", savenum);
            PlayerPrefs.SetInt(savenum + "isSaved", 1);
            PlayerPrefs.SetString(savenum + "Save", saveJString);
            PlayerPrefs.Save();
            BlinkSLMarker(0);
        }
    }


    public void SkipDown()
    {
        skipping = true;
        sc = StartCoroutine(SkipCoroutine());
    }


    IEnumerator SkipCoroutine()
    {
        while (skipping && CoroutinesWorking.Count == 0 && !loading && !menu)
        {
            Step();
            yield return new WaitForSeconds(0.25f);

        }
            
      
    }
    public void SkipUp()
    {
        skipping = false;
    }
    public void Step()
    {
        if (!QTE && !hideText && !loading && CoroutinesWorking.Count==0)
        {
            if (labelNum >= scenario.Count)
            {
                Debug.Log("Стоп");
                //TODO Конец игры
            }
            else
            {
                if (lineNum >= scenario[labelNum].lines.Count)
                {
                    lineNum = 0;
                    labelNum++;
                    Step();
                }
                else
                {
                    LineTypeAction(scenario[labelNum].lines[lineNum]);
                }
            }
        }
    }

    void LineTypeAction(Item line)
    {
        if (cid > 100)
            cid = 0;
        switch (line.type)
        {
            case GDB.LineType.Line:
                textAction(line, false); 
                break;
            case GDB.LineType.FScreen:
                FSTextAction(line, false); 
                break;
            case GDB.LineType.Chat:
                chatAction(line, false); 
                break;
            case GDB.LineType.Jump:
                jumpAction(line);
                break;
            case GDB.LineType.Var:
                varAction(line);
                break;
            case GDB.LineType.If:
                ifAction(line);
                break;
            case GDB.LineType.Menu:
                menuAction(line);
                break;
            case GDB.LineType.CG:
                cgAction(line, false);
                break;
            case GDB.LineType.BG:
                bgAction(line, false);
                break;
            case GDB.LineType.Music:
                musicAction(line, false);
                break;
            case GDB.LineType.Sound:
                soundAction(line, false);
                break;
            case GDB.LineType.Pause:
                pauseAction(line);
                break;
            case GDB.LineType.Actor:
                actorAction(line, false);
                break;
            case GDB.LineType.CamEffect:
                cameraAction(line);
                break;
            case GDB.LineType.Emoji:
                emojiAction(line);
                break;
            case GDB.LineType.Investigation:
                invAction(line);
                break;
            case GDB.LineType.Event:
                eventAction(line);
                break;
        }
    }
    void invAction(Item line)
    {
       
           
            chatManager.Disable();
            FSPanel.SetActive(false);
            switch (line.inv)
            {
                case GDB.Investigation.Open:
                {
 				if (!isGameRunning)
                 {
 					isGameRunning = true;
                    Camera.gameObject.SetActive(false);
                    textCanvas.enabled = false;
                    List<int> unlockedThoughts = new List<int>();
                    if (saveObj.unlockedInv.Count > 0)
                        foreach (UnlockMessage item in saveObj.unlockedInv)
                        {
                            if (item.InvName == line.additionalPose)
                            {
                                unlockedThoughts.Add(item.ID);
                            }
                        }

                    invController.SetNewGame(line.additionalPose, unlockedThoughts, saveObj.drugsCount);
                    Debug.Log("SETTED");
                }}
                    break;
                case GDB.Investigation.AddThought:
                {

                    StartCoroutine(NewThoughtCoroutine("Идея!", cid++));

                    saveObj.unlockedInv.Add(new UnlockMessage(line.additionalPose, line.value));

                    //Debug.Log(saveObj.unlockedInv[0]);

                    /*
                     SaveObject saveData;
                    saveData = new SaveObject();
                    saveData.unlockedInv.Add(new UnlockMessage("Unlocked feature A", 1));
                    saveData.unlockedInv.Add(new UnlockMessage("Unlocked feature B", 2));
    
                    string jsonData = JsonUtility.ToJson(saveData);
                    Debug.Log("Serialized SaveObject to JSON:\n" + jsonData);
    
                    // Здесь можно сохранить jsonData в файл или другое хранилище
                    // Предположим, что у вас уже есть JSON-строка с данными
    
                    SaveObject loadedData = JsonUtility.FromJson<SaveObject>(jsonData);
                    saveData = loadedData;
                    Debug.Log(saveData.unlockedInv[0]);
                  
                */



                }
                    break;
                case GDB.Investigation.AddDrugs:
                {
                    StartCoroutine(NewThoughtCoroutine("Еще один стимулятор", cid++));
                    saveObj.drugsCount++;


                }
                    break;




            
        }
    }


	void eventAction(Item line)
    {
    
        if(!skipping){
           chatManager.Disable();
           FSPanel.SetActive(false);
           StartCoroutine(EventCoroutine(line, cid++));
        }
    }

	IEnumerator EventCoroutine (Item line , int id){
		CoroutinesWorking.Add(id);
      
      
      ResourceRequest request = Resources.LoadAsync<GameObject>("Events/"+line.additionalPose.ToString());
                   
       while (!request.isDone)
       {
           yield return null;
       }

       if (request.asset == null)
       {
           Debug.LogError("Failed to load evevnt at path: Events/" + line.additionalPose.ToString());
       }
       else
       {
           GameObject obj = request.asset as GameObject;
           // Делаем что-то с загруженным спрайтом
           
       
           GameObject.Instantiate(obj, EventCanvas);
           Debug.Log("Event loaded successfully!");
       }
      
      
       
           CoroutinesWorking.Remove(id);
       
            lineNum++;

        

	}
    public void UseDrugs()
    {
        saveObj.drugsCount--;
    }
    IEnumerator NewThoughtCoroutine(string text, int cid)
    {
        CoroutinesWorking.Add(cid);
        investigationLabel.SetActive(true);
        investigationLabel.GetComponentInChildren<TMP_Text>().text = text;
        yield return new WaitForSeconds(2);
        investigationLabel.SetActive(false);
        CoroutinesWorking.Remove(cid);
        lineNum++;
        Step();    
    }
   

    public void EndInv()
    {
        Camera.gameObject.SetActive(true);
        textCanvas.enabled = true;
        isGameRunning = false;
    }
    
    void cameraAction(Item line)
    {
        chatManager.Disable();
        FSPanel.SetActive(false);
        StartCoroutine(EffectCoroutine(line, cid++));


    }

    
    void emojiAction(Item line)
    {
        chatManager.Disable();
        FSPanel.SetActive(false);
        if (Emoji!=null)
            Destroy(Emoji);
        GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
        if (ActorOnScene != null)
        { 
            Emoji = Instantiate(emojiList[(int)line.emoji], ActorOnScene.transform.position + Vector3.up * 4, transform.rotation);
         
            
        }
        lineNum++;
        Step();
    }

    IEnumerator EffectCoroutine(Item line, int cid)
    {
        CoroutinesWorking.Add(cid);
        ApplyEffects(line.effects, true, line.time, line.V3position);
        yield return new WaitForSeconds(1f);
        ApplyEffects(line.effects, false, line.time, line.V3position);
        lineNum++;
        CoroutinesWorking.Remove(cid);
        Step();
    }

    void actorAction(Item line, bool isLoad)
    {
       
           StartCoroutine(LoadActor(line, cid++, isLoad));


    }
    GameObject FindActorOnScene(string name)
    {
        foreach(Actor obj in actorsOnScene)
        {
            if (obj.Name.ToString() == name)
            {
                return obj.obj;
            }
        }

        return null;
    }
    void RemoveActor(string name)
    {
        foreach(Actor obj in actorsOnScene)
        {
            if (obj.Name.ToString() == name)
            {
                actorsOnScene.Remove(obj);
                return;
            }
        }

    
    }
    Actor FindActorOnScenePointer(string name)
    {
        foreach (Actor obj in actorsOnScene)
        {
            if (obj.Name.ToString() == name)
            {
                return obj;
            }
        }

        return null;
    }

    IEnumerator LoadActor(Item line, int id, bool isLoad)
    {
        CoroutinesWorking.Add(id);
        GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
        if (ActorOnScene != null)
        {
            Destroy(ActorOnScene);
        }

        RemoveActor(line.name.ToString());


        ResourceRequest request = Resources.LoadAsync<GameObject>("Actors/"+line.name.ToString() + line.pose.ToString());
                               
        while (!request.isDone)
        {
             yield return null;
        }
                               
        if (request.asset == null)
        {                                       
             Debug.LogError("Failed to load actor at path: Actors/" + line.name.ToString() + line.pose.ToString());
        }
        else
        {
             GameObject obj = request.asset as GameObject;
             ActorOnScene = GameObject.Instantiate(obj);



             Actor actor;
 if (line.name == GDB.Name.Женя){
 actor = new Actor(line.V3position, GDB.Name.Соня, line.pose);}
             if (line.name == GDB.Name.Мира && FindVariable(GDB.Variables.Injure) >=0 && saveObj.Variables[FindVariable(GDB.Variables.Injure)].varValue > 0)
                 actor = new Actor(line.V3position, GDB.Name.Мира_травма, line.pose);
             else
             {
                 actor = new Actor(line.V3position, line.name, line.pose);
             }
             actor.obj = ActorOnScene;
             actorsOnScene.Add(actor);
             Debug.Log("Actor loaded successfully!");
         }
                  
            
            
            
            if (Emoji != null)
                Destroy(Emoji);


        

        ActorOnScene.transform.position = line.V3position;
        //ActorOnScene.GetComponent<Animator>().WriteDefaultValues();

        ActorOnScene.GetComponent<Animator>().SetBool(line.spriteEffect.ToString(), true);
        yield return new WaitForSeconds(0.1f);
        ActorOnScene.GetComponent<Animator>().SetBool(line.spriteEffect.ToString(), false);

        if (line.pose == GDB.Pose.Hide || line.spriteEffect == GDB.SpriteEffect.DissolveOut)
        {
            actorsOnScene.Remove(FindActorOnScenePointer(line.name.ToString()));
            if (!skipping)
            {

                yield return new WaitForSeconds(1);

            }

            try
            {
                Destroy(ActorOnScene);
            }
            catch
            {
            }
        }
        else
        {
            FindActorOnScenePointer(line.name.ToString()).Pose = line.pose;
        }
        CoroutinesWorking.Remove(id);
        if (!isLoad)
        {
            lineNum++;
            Step();
        }

        
        
      
    }









    void pauseAction(Item line)
    {
      
        if (!skipping)
        {
           
            StartCoroutine(PauseCoroutine(line, cid++));
        }
        else
        {
            lineNum++;
            Step();
        }
    }


    IEnumerator PauseCoroutine(Item line, int cid)
    {
            CoroutinesWorking.Add(cid);
            yield return new WaitForSeconds(line.time);
            lineNum++;
            CoroutinesWorking.Remove(cid);
            Step();
      
    }
    void soundAction(Item line, bool isLoad)
    {

        if (line.show) {

            if (loadingSound)
            {
                try { StopCoroutine(lScoroutine); } catch { }
                loadingSound = false;
            }
            lScoroutine = StartCoroutine(LoadSound(line, cid++, isLoad));

        }
            
        else
        {
            soundPlayer.Stop();
            if (!isLoad)
            {
                lineNum++;
                Step();
            }
        }
    
    }
    IEnumerator LoadSound(Item line, int cid, bool isLoad)
    {
        CoroutinesWorking.Add(cid);
        loadingSound = true;
        
        
        
        
         ResourceRequest request = Resources.LoadAsync<AudioClip>("Sounds/"+line.additionalPose);
                                       
                                               while (!request.isDone)
                                               {
                                                   yield return null;
                                               }
                                       
                                               if (request.asset == null)
                                               {
                                                   Debug.LogError("Failed to load sound at path: Sounds/" +line.additionalPose);
                                               }
                                               else
                                               {
                                               AudioClip res = request.asset as AudioClip;
                                               soundPlayer.PlayOneShot(res);
                                               
                                                           
                                               }
        
        

        
        if (!isLoad)
                                                                   {
                                                                       lineNum++;
                                                                       Step();
                                                                   }
        
        
        
        
        
        
        loadingSound = false;
              CoroutinesWorking.Remove(cid);
    }
    void musicAction(Item line, bool isLoad)
    {
        
        if (line.show) {
            if (loadingMusic)
            {
                try { StopCoroutine(lMcoroutine); } catch { }
                loadingMusic = false;
            }
            lMcoroutine = StartCoroutine(LoadMusic(line, cid++, isLoad));
        }
          
       else
        {
            fadeOutMusic = fadeInMusic; 
            fadeInMusic = null;
            if (fadeOutMusic)
                fadeOutMusic.GetComponent<Fader>().fadingOut = true;

            if (!isLoad)
            {
                lineNum++;
                Step();
            }
        }
    }
    IEnumerator LoadMusic(Item line, int cid, bool isLoad)
    {
        CoroutinesWorking.Add(cid);
        saveObj.lastMusic = line.music;
        if (fadeInMusic)
            fadeOutMusic = fadeInMusic;
        fadeInMusic = Instantiate(musicPlayer, transform).GetComponent<AudioSource>();




 ResourceRequest request = Resources.LoadAsync<AudioClip>("Music/"+line.music.ToString());
                               
        while (!request.isDone)
        {
             yield return null;
        }
                               
        if (request.asset == null)
        {                                       
             Debug.LogError("Failed to load music at path: Music/" + line.music.ToString());
        }
        else
        {
             AudioClip res = request.asset as AudioClip;
         
            fadeInMusic.clip = res;
            fadeInMusic.Play();
            fadeInMusic.GetComponent<Fader>().maxVolumeMusic = maxVolumeMusic;
            if (fadeOutMusic)
                fadeOutMusic.GetComponent<Fader>().fadingOut = true;
            fadeInMusic.GetComponent<Fader>().fadingIn = true;

             Debug.Log("Music loaded successfully!");
         }



       if (!isLoad)
            {
                lineNum++;
                Step();
            }


       
        
        //Design
        if (!skipping)
        {
            musicIcon.SetBool("Play", true);
            musicName.text = TrackList[(int) line.music].Name;
            musicAuthor.text = TrackList[(int) line.music].Author;
            yield return new WaitForSeconds(0.5f);
            musicIcon.SetBool("Play", false);
        } 

        CoroutinesWorking.Remove(cid);
    }
    void bgAction(Item line, bool isLoad)
    {
    
        if (Emoji != null)
         Destroy(Emoji);
        Debug.Log(("Попытка загрузки BG"));  
        ClearSprites();
        if (loadingBG)
        {
            try { StopCoroutine(lBGcoroutine); } catch { }
            loadingBG = false;
        }
        lBGcoroutine = StartCoroutine(LoadBackground(line, cid++, isLoad));

    }

    IEnumerator LoadBackground(Item line, int cid, bool isLoad)
    {
        CoroutinesWorking.Add(cid);
        Camera.SetBool("BlackOut", true);
        yield return new WaitForSeconds(0.5f);
        saveObj.lastBG = line.BGname;
        saveObj.lastCG = "";
        loadingBG = true;


 ResourceRequest request = Resources.LoadAsync<VideoClip>("3DBG/"+line.BGname.ToString());
                               
        while (!request.isDone)
        {
             yield return null;
        }
                               
        if (request.asset == null)
        {                                       
             Debug.LogError("Failed to load video at path: 3DBG/" + line.BGname.ToString());
        }
        else
        {
             VideoClip res = request.asset as VideoClip;
            ClearBG();
            BG.gameObject.GetComponent<MeshRenderer>().enabled = true;
            CG.enabled = false;
            clipBG = res;
            BG.clip = clipBG;
            BG.Play();
            if (!isLoad)
                lineNum++;
          
            yield return new WaitForSeconds(1f);
            Camera.SetBool("BlackOut", false);
            if (line.effects != GDB.Effects.BlackOut)
            {
                ApplyEffects(line.effects, true, line.time, line.V3position);
                yield return new WaitForSeconds(1.5f);
                ApplyEffects(line.effects, false, line.time, line.V3position);
            }
             Debug.Log("Video loaded successfully!");
         }




  





        loadingBG = false;
        CoroutinesWorking.Remove(cid);
        if (!isLoad)
            Step();
      

    }

    void ClearBG()
    {
        BG.clip = null;
    }

    void ApplyEffects(GDB.Effects effect, bool show, float zoom, Vector3 dest)
    {
        switch (effect){
            case GDB.Effects.Zoom:
                {
                    GetComponent<CameraMoveZoom>().NewMove(new Vector3 (0, 0, 0),zoom );

                } break;
            case GDB.Effects.PointTo:
                {
                    Camera.GetComponent<Animator>().enabled = false;
                             GetComponent<CameraMoveZoom>().NewMove(dest,45.45f );

                }
                break;
            case GDB.Effects.PointToAndZoom:
                {
                    Camera.GetComponent<Animator>().enabled = false;
                             GetComponent<CameraMoveZoom>().NewMove(dest,zoom);

                }
                break;
            default: 
                {
                    Camera.GetComponent<Animator>().enabled = true;
                    Camera.SetBool(effect.ToString(), show);
                } break;
        }
     
    }
    
    void cgAction(Item line, bool isLoad)
    {
        
        if (line.CGname != ""){
        chatManager.Disable();
        FSPanel.SetActive(false);
        if (Emoji != null)
            Destroy(Emoji);
        PlayerPrefs.SetInt(line.CGname, 1);

        if (loadingCG)
        {
            try { StopCoroutine(lCGcoroutine); } catch { }
            loadingCG = false;
        }
        lCGcoroutine = StartCoroutine(LoadSprite(line, isLoad));
       // Step();

     //TODO Эффекты
        }
    }

    IEnumerator LoadSprite(Item line, bool isLoad)
    {
      
            
            CoroutinesWorking.Add(cid);
            Camera.SetBool("BlackOut", true);
            yield return new WaitForSeconds(0.5f);


            if (line.show)
            {
                //TODO ЭФфекты
                loadingCG = true;
             
             
             
             
                     ResourceRequest request = Resources.LoadAsync<Sprite>("CG/"+line.CGname);
             
                     while (!request.isDone)
                     {
                         yield return null;
                     }
             
                     if (request.asset == null)
                     {
                         Debug.LogError("Failed to load sprite at path: CG/" + line.CGname);
                     }
                     else
                     {
                         Sprite sprite = request.asset as Sprite;
                         // Делаем что-то с загруженным спрайтом
                         
                    saveObj.lastCG = line.CGname;
                    saveObj.lastBG = GDB.BGName.None;
                    BG.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    CG.enabled = true;
                    CG.sprite = sprite;
                         Debug.Log("Sprite loaded successfully!");
                     }
             
             
             
             
            
          
                if (!isLoad)
                    lineNum++;
                loadingCG = false;
            }
            else
            {
                if (!skipping)
                {
                    //TODO ЭФфекты
                    yield return new WaitForSeconds(1);

                }

                saveObj.lastCG = "";
                if (!isLoad)
                    lineNum++;
                CG.enabled = false;
            }

            yield return new WaitForSeconds(1f);
            Camera.SetBool("BlackOut", false);
            if (line.effects != GDB.Effects.BlackOut && line.effects != GDB.Effects.HSlide && line.effects != GDB.Effects.VSlide)
            {
                ApplyEffects(line.effects, true, line.time, line.V3position);
                yield return new WaitForSeconds(1.5f);
                ApplyEffects(line.effects, false, line.time, line.V3position);
            }
            if (line.effects == GDB.Effects.HSlide || line.effects == GDB.Effects.VSlide)
            {
           
                ApplyEffects(line.effects, true, line.time, line.V3position);
                yield return new WaitForSeconds(3f);
                ApplyEffects(line.effects, false, line.time, line.V3position);
            
            }
            CoroutinesWorking.Remove(cid);
            if (!isLoad)
                Step();
        
    }
    public void decisionAction(string name)
    {
        menuTimerSlider.SetActive(false);
        chatManager.Disable();
        FSPanel.SetActive(false);
        if (!choiseRoulette)
        {
            try
            {
                StopCoroutine(menuCoroutine);
            }
            catch
            {
            }

            foreach (var cb in choiseBoxes)
            {
                Destroy(cb);
            }

            textCanvas.enabled = true;
            choiseBoxes.Clear();
            Item item = new Item();
            item.additionalPose = name;
            jumpAction(item);
        }

        menu = false;
    }
    void menuAction(Item line)
    {
        menu = true;
        Save("Auto");
        chatManager.Disable();
        FSPanel.SetActive(false);
        menuCoroutine = StartCoroutine(MenuActionCoroutine());
        textCanvas.enabled = false;
        foreach (var choiseVariant in line.menu_label)
        {
            if (choiseVariant!= ""){
                ChoiseBox cb;
                cb = Instantiate(choiseBox, choiseGroup.transform).GetComponent<ChoiseBox>();
                cb.SetMenuItem(choiseVariant, line.menu_jump[line.menu_label.IndexOf(choiseVariant)], this);
                choiseBoxes.Add(cb.gameObject);
            }
            
        }
    }

    IEnumerator MenuActionCoroutine()
    {
        
        skipping = false;
        if (sc!=null)
            StopCoroutine(sc);
        menuTimerSlider.SetActive(true);
        Slider slider = menuTimerSlider.GetComponent<Slider>();
        float timer = 1.0f;
        slider.value = timer;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
            timer -= 0.2f;
            slider.value = timer;
        }
        menuTimerSlider.SetActive(false);
        
        //Roulette
        choiseRoulette = true;
        
        int random = Random.Range(0, choiseBoxes.Count);      
        Debug.Log("Random number is... " + random);

        for (int i = 0; i < 2; i++)
        {
            foreach (GameObject box in choiseBoxes)
            {
                Vector3 oldScale = box.transform.localScale;
                box.transform.localScale *= 1.2f;
                yield return new WaitForSeconds(0.2f);
                box.transform.localScale = oldScale;
            }
            
        }

        for (int i = 0; i <= random; i++)
        {
            GameObject box = choiseBoxes[i];
            Vector3 oldScale = box.transform.localScale;
            box.transform.localScale *= 1.2f;
            yield return new WaitForSeconds(0.2f);
            box.transform.localScale = oldScale;
            
        }
        choiseBoxes[random].transform.localScale *= 1.2f;
        yield return new WaitForSeconds(0.7f);
        choiseRoulette = false;
        choiseBoxes[random].GetComponent<ChoiseBox>().MakeDecision();

        
    }

    void ifAction(Item line)
    {
        //chatManager.Disable();
        //FSPanel.SetActive(false);
        int foundVar = FindVariable(line.var);
        if (foundVar >= 0){
            int var = saveObj.Variables[foundVar].varValue;
            Debug.Log(line.var);
            Debug.Log(var);
            switch (line.signsIf)
            {
                case GDB.SignsIf.greater:
                {
                    if (var > line.value)
                        jumpAction(line);
                    else
                    {
                        lineNum++;
                        Step();
                    }
                }
                    break;
                case GDB.SignsIf.less:
                {
                    if (var < line.value)
                        jumpAction(line);
                    else
                    {
                        lineNum++;
                        Step();
                    }
                }
                    break;
                case GDB.SignsIf.equal:
                {
                    if (var == line.value)
                        jumpAction(line);
                    else
                    {
                        lineNum++;
                        Step();
                    }

                }
                    break;
                default:
                    Step();
                    break;
            }


        }
    }

    void varAction(Item line)
    {
        
        StartCoroutine(NewThoughtCoroutine("Они это запомнят", cid++));
        //Debug.Log(line.value); 
        switch (line.signs)
        {
            case GDB.Signs.decr:
            {
                int i = FindVariable(line.var); 
                if (i>-1)
                    saveObj.Variables[i].varValue--;
                
                
            }
                break;
            case GDB.Signs.incr: 
           {
               int i = FindVariable(line.var); 
               if (i>-1)
                   saveObj.Variables[i].varValue++;
               else
               {
                   saveObj.Variables.Add(new VarItem(line.var, 1));
               }

            }
                break;
            case GDB.Signs.equal:
            {
               
                int i = FindVariable(line.var);
                if (i > -1)
                    saveObj.Variables[i].varValue = line.value;
                else
                {
                    saveObj.Variables.Add(new VarItem(line.var, line.value));
                }

            }
                break;
        }
    
        
    }

    public void UpdateVar( GDB.Variables var, int value)
    {
        int i = FindVariable(var);
        if (i > -1)
            saveObj.Variables[i].varValue = value;
        else
        {
            saveObj.Variables.Add(new VarItem(var, value));
        }
    }

    int FindVariable(GDB.Variables name)
    {
        foreach (var item in saveObj.Variables)
        {
            if (item.varName == name)
            {
                return saveObj.Variables.IndexOf(item);
            } 
        }

        return -1;
    }

    int FindLabel(string name)
    {
        foreach (var label in scenario)
        {
            if (label.name == name)
            {
                return scenario.IndexOf(label);
            } 
        }

        return -1;
    }
    void jumpAction(Item line)
    {
       
        labelNum = FindLabel(line.additionalPose);
        lineNum = 0;
        Step();
    }
    public void jumpAction(string name)
    {
       
        labelNum = FindLabel(name);
        lineNum = 0;
        Step();
    }
    void ApplyTextEffects(GDB.Fonts font) {
        
        textContent.GetComponent<TextEffects>().effect = font;
        
    }
     void chatAction(Item line, bool isLoad)
    {
        textContent.text = "";
        textFS.text = "";
        textAuthor.text = "";
        FSPanel.SetActive(false);
        Log.RenewLog(line.name, line.line);
        saveObj.lastAuthor = line.name;
        saveObj.lastLine = line.line;
        saveObj.textDisplay = GDB.TextDisplay.Chat;
        if (line.name == GDB.Name.Мира)
            chatManager.SendMessage(line.line, line.name, false);
        else
            chatManager.SendMessage(line.line, line.name, true);
    
        if (!isLoad)
            lineNum++;

       
    }
    void textAction(Item line, bool isLoad)
    {               
        
        chatManager.Disable();
        FSPanel.SetActive(false);
        if (saveObj.lastLine != textContent.text && !isLoad && saveObj.textDisplay == GDB.TextDisplay.Dialogue)
        {
            saveObj.textDisplay = GDB.TextDisplay.Dialogue;
            textContent.text = saveObj.lastLine;
            if (c != null)
            {
          
                SetTextMarker(false);
                StopCoroutine(c);
            }
        }
        else
        {
            saveObj.textDisplay = GDB.TextDisplay.Dialogue;
            foreach (var actor in actorsOnScene)
            {
                actor.obj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.gray;
            }
            GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
            if (line.name==GDB.Name.Мира && ActorOnScene == null)
            {
               ActorOnScene = FindActorOnScene(GDB.Name.Мира_травма.ToString());
            }
            if (line.name==GDB.Name.Женя && ActorOnScene == null)
            {
               ActorOnScene = FindActorOnScene(GDB.Name.Соня.ToString());
            }
            if (ActorOnScene != null)
            {
                
                ActorOnScene.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.white;
            }
            else
            {
                foreach (var actor in actorsOnScene)
                {
                    actor.obj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.gray;
                }
            }
       
     
            ApplyTextEffects(GDB.Fonts.Regular);
            saveObj.lastAuthor = line.name;
            saveObj.lastLine = line.line;
        
            if (c != null)
            {
          
                SetTextMarker(false);
                StopCoroutine(c);
            }

            castingLine = "";
            textContent.text = "";

            textAuthor.color = GDB.CharColor((int)line.name);
       
            Log.RenewLog(line.name, line.line);

            ApplyTextEffects(line.font);

            if (!skipping && textDelay > 0)
            {
                castingLine = line.line;
           
                c = StartCoroutine(textCastEnum(textContent));
            }
            else
            {
            
                textContent.text = line.line;
            }
            Debug.Log(line.name.ToString());
            textAuthor.text = line.name.ToString().Replace("_", "-").Replace("Мира-травма", "Мира");
            if (!isLoad)
                lineNum++;
        }
        
        
       
    }
    void FSTextAction(Item line, bool isLoad)
    {
        FSPanel.SetActive(true);
        saveObj.textDisplay = GDB.TextDisplay.Fullscreen;
        chatManager.Disable();
        
        textContent.text = "";
        textFS.text = "";
        textAuthor.text = "";

        saveObj.lastAuthor = line.name;
        saveObj.lastLine = line.line;

        castingLine = "";

        Log.RenewLog(line.name, line.line);

        if (!skipping && textDelay > 0)
        {
            castingLine = line.line;
            c = StartCoroutine(textCastEnum(textFS));
        }
        else
        {
        
            textFS.text = line.line;
        }
        Debug.Log(line.name.ToString());
        if (!isLoad)
            lineNum++;
        
        
        
       
    }
    private IEnumerator textCastEnum(TextMeshProUGUI textContent)
    {
        SetTextMarker(true);
        foreach (var _char in castingLine)
        {
            textContent.text += _char;
            yield return new WaitForSeconds(textDelay);
        }
        SetTextMarker(false);
        
    }
}
