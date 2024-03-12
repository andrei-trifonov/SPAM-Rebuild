using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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
    [SerializeField] private Animator Camera;
    
    [SerializeField] private Dialogue scenarioComposer;
    private List<LabelSample> scenario;
    
    private float maxVolumeMusic = 1; //TODO
    private Coroutine c;
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
    
    public List<int> CoroutinesWorking;
    int cid;
    public SaveObject saveObj = new SaveObject();
    private string saveJString = "";

    private GameObject Emoji;
    [SerializeField] private List<GameObject> emojiList = new List<GameObject>();
    
    public void SetTextMarker(bool state)
    {   
        textCanvasAnimator.SetBool("Ready", !state);
    }
    public void EnableText()
    {
        hideText = !textCanvasAnimator.GetBool("Hide");
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
        TrackList = GetComponent<OSTList>().GetTrackList();
        saveObj = new SaveObject();
        textCanvasAnimator = textCanvas.GetComponent<Animator>();
        scenario = scenarioComposer.Labels;
        Load(PlayerPrefs.GetString("LastSave"));
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
        saveLoadMarker.transform.GetChild(state).gameObject.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        saveLoadMarker.SetActive(false);
        saveLoadMarker.transform.GetChild(0).gameObject.SetActive(false);
        saveLoadMarker.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void Load(string savenum)
    {
        if (!loading && CoroutinesWorking.Count == 0)
        {
            //Clean scene
            StopAllCoroutines();
            loading = true;
            skipping = false;

            ClearSprites();
            CG.enabled = false;
            ClearBG();
                
            
            ApplyTextEffects(GDB.Fonts.Regular);
            textContent.text = "";
            textAuthor.text = "";
                

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
                musicAction(item);

            //Get 
            foreach (var actor in loadedData.Actors)
            {
                item.pose = actor.Pose;
                item.name = actor.Name;
                item.V3position = actor.Position;
                actorAction(item);
            }
           
           
            //Spawn Scene
            
            item.BGname = loadedData.lastBG;
            if (loadedData.lastBG!= GDB.BGName.None)
                bgAction(item);
            
            item.CGname = loadedData.lastCG;
            if (loadedData.lastCG!="")
                cgAction(item);
            //Return line num label num
            lineNum = loadedData.lastLineNum;
            labelNum = loadedData.lastLabelNum;


            //return text to panelx 

            item.line= loadedData.lastLine;
            item.name = loadedData.lastAuthor;
            textAction(item);
            
            BlinkSLMarker(1);
            loading = false;
            
          
        }
    }
    public void Save(string savenum)
    {
        if (!loading && CoroutinesWorking.Count == 0)
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
            saveJString = JsonUtility.ToJson(saveObj);
            
            Debug.Log("Actors "  + saveObj.Actors);
            Debug.Log("AuthorText "  + saveObj.lastAuthor);
            Debug.Log("LineText "  + saveObj.lastLine);
            Debug.Log("Scene "  + saveObj.lastBG);
            Debug.Log("CG "  + saveObj.lastBG);
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
        StartCoroutine(SkipCoroutine());
    }


    IEnumerator SkipCoroutine()
    {
        while (skipping && CoroutinesWorking.Count == 0 && !loading)
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
        if (!hideText && !loading && CoroutinesWorking.Count==0)
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
                textAction(line); 
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
                cgAction(line);
                break;
            case GDB.LineType.BG:
                bgAction(line);
                break;
            case GDB.LineType.Music:
                musicAction(line);
                break;
            case GDB.LineType.Sound:
                soundAction(line);
                break;
            case GDB.LineType.Pause:
                pauseAction(line);
                break;
            case GDB.LineType.Actor:
                actorAction(line);
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
        }
    }
    void invAction(Item line)
    {
        switch (line.inv)
        {
            case GDB.Investigation.Open:
            {
                Camera.gameObject.SetActive(false);
                textCanvas.enabled = false;
                List<int> unlockedThoughts = new List<int>();
                if (saveObj.unlockedInv.Count>0)
                foreach (UnlockMessage item in saveObj.unlockedInv)
                {
                    if (item.InvName == line.additionalPose)
                    {
                        unlockedThoughts.Add(item.ID);
                    }
                }

            
                invController.SetNewGame(line.additionalPose, unlockedThoughts, saveObj.drugsCount);
                 //TODO
            }
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
    }
    
    void cameraAction(Item line)
    {

        StartCoroutine(EffectCoroutine(line, cid++));


    }

    
    void emojiAction(Item line)
    {
        if (Emoji!=null)
            Destroy(Emoji);
        GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
        if (ActorOnScene != null)
        { 
            Emoji = Instantiate(emojiList[(int)line.emoji], ActorOnScene.transform.position + Vector3.up * 3, transform.rotation);
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

    void actorAction(Item line)
    {
       
           StartCoroutine(LoadActor(line, cid++));


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
    IEnumerator LoadActor(Item line, int id)
    {
        CoroutinesWorking.Add(id);
        GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
        if (ActorOnScene == null)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(line.name.ToString()+line.pose.ToString());
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject res = handle.Result;
                ActorOnScene = GameObject.Instantiate(res);
                Actor actor = new Actor(Vector3.zero, line.name, line.pose);
                actor.obj = ActorOnScene;
              
                actorsOnScene.Add(actor);

            }
            Addressables.Release(handle); 
           
            
        }
        
        ActorOnScene.transform.position = line.V3position;
        ActorOnScene.GetComponent<Animator>().WriteDefaultValues();
       
        ActorOnScene.GetComponent<Animator>().SetBool(line.pose.ToString(), true);
        if (line.pose == GDB.Pose.Hide)
        {
            actorsOnScene.Remove(FindActorOnScenePointer(line.name.ToString()));
            if (!skipping)
            {

                yield return new WaitForSeconds(1);

            }
            try { Destroy(ActorOnScene); }
            catch { }
        }
        else {
            FindActorOnScenePointer(line.name.ToString()).Pose = line.pose;
        }


        lineNum++;
        CoroutinesWorking.Remove(id);
        //Step();

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
    void soundAction(Item line)
    {

        if (line.show) {

            if (loadingSound)
            {
                try { StopCoroutine(lScoroutine); } catch { }
                loadingSound = false;
            }
            lScoroutine = StartCoroutine(LoadSound(line, cid++));

        }
            
        else
        {
            soundPlayer.Stop();
            lineNum++;
            Step();
        }
    
    }
    IEnumerator LoadSound(Item line, int cid)
    {
        CoroutinesWorking.Add(cid);
        loadingSound = true;
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(line.additionalPose);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip res = handle.Result;
            
            soundPlayer.PlayOneShot(res);
            lineNum++;
            
            Step();
        }

        Addressables.Release(handle);
        loadingSound = false;
        CoroutinesWorking.Remove(cid);
    }
    void musicAction(Item line)
    {
        if (line.show) {
            if (loadingMusic)
            {
                try { StopCoroutine(lMcoroutine); } catch { }
                loadingMusic = false;
            }
            lMcoroutine = StartCoroutine(LoadMusic(line, cid++));
        }
          
       else
        {
            fadeOutMusic = fadeInMusic; 
            fadeInMusic = null;
            if (fadeOutMusic)
                fadeOutMusic.GetComponent<Fader>().fadingOut = true;
            lineNum++;
            Step();
        }
    }
    IEnumerator LoadMusic(Item line, int cid)
    {
        CoroutinesWorking.Add(cid);
        saveObj.lastMusic = line.music;
        if (fadeInMusic)
            fadeOutMusic = fadeInMusic;
        fadeInMusic = Instantiate(musicPlayer, transform).GetComponent<AudioSource>();
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(line.music.ToString());
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip res = handle.Result;
            fadeInMusic.clip = res;
            fadeInMusic.Play();
            fadeInMusic.GetComponent<Fader>().maxVolumeMusic = maxVolumeMusic;
            if (fadeOutMusic)
                fadeOutMusic.GetComponent<Fader>().fadingOut = true;
            fadeInMusic.GetComponent<Fader>().fadingIn = true;
            lineNum++;
            
            Step();
        }
        Addressables.Release(handle);
        
        //Design
        musicIcon.SetBool("Play", true);
        musicName.text = TrackList[(int)line.music].Name;
        musicAuthor.text = TrackList[(int)line.music].Author;
        yield return new WaitForSeconds(0.5f);
        musicIcon.SetBool("Play", false);   
        //

        CoroutinesWorking.Remove(cid);
    }
    void bgAction(Item line)
    {
        Debug.Log(("Попытка загрузки BG"));  
        ClearSprites();
        if (loadingBG)
        {
            try { StopCoroutine(lBGcoroutine); } catch { }
            loadingBG = false;
        }
        lBGcoroutine = StartCoroutine(LoadBackground(line, cid++));

    }

    IEnumerator LoadBackground(Item line, int cid)
    {
        CoroutinesWorking.Add(cid);
        Camera.SetBool("BlackOut", true);
        yield return new WaitForSeconds(0.5f);
        saveObj.lastBG = line.BGname;
        saveObj.lastCG = "";
        loadingBG = true;
        AsyncOperationHandle<VideoClip> handle = Addressables.LoadAssetAsync<VideoClip>(line.BGname.ToString());
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            ClearBG();
            
            clipBG = handle.Result;
            BG.clip = clipBG;
            BG.Play();
            lineNum++;
          

        }
        yield return new WaitForSeconds(1f);
        Camera.SetBool("BlackOut", false);
        if (line.effects != GDB.Effects.BlackOut)
        {
            ApplyEffects(line.effects, true, line.time, line.V3position);
            yield return new WaitForSeconds(1.5f);
            ApplyEffects(line.effects, false, line.time, line.V3position);
        }
        Addressables.Release(handle);
        loadingBG = false;
        CoroutinesWorking.Remove(cid);
      

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
                    GetComponent<CameraMoveZoom>().destZoom = zoom;
                } break;
            case GDB.Effects.PointTo:
                {
                    Camera.GetComponent<Animator>().enabled = false;
                    GetComponent<CameraMoveZoom>().destPos = dest;
                }
                break;
            case GDB.Effects.PointToAndZoom:
                {
                    Camera.GetComponent<Animator>().enabled = false;
                    GetComponent<CameraMoveZoom>().destPos = dest;
                    GetComponent<CameraMoveZoom>().destZoom = zoom;
                }
                break;
            default: 
                {
                    Camera.GetComponent<Animator>().enabled = true;
                    Camera.SetBool(effect.ToString(), show);
                } break;
        }
     
    }
    
    void cgAction(Item line)
    {
        PlayerPrefs.SetInt(line.CGname, 1);

        if (loadingCG)
        {
            try { StopCoroutine(lCGcoroutine); } catch { }
            loadingCG = false;
        }
        lCGcoroutine = StartCoroutine(LoadSprite(line));
     

     //TODO Эффекты 
    }

    IEnumerator LoadSprite(Item line)
    {
        CoroutinesWorking.Add(cid);
        Camera.SetBool("BlackOut", true);
        yield return new WaitForSeconds(0.5f);

       
        if (line.show)
        {
            //TODO ЭФфекты
            loadingCG = true;
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(line.CGname);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                saveObj.lastCG = line.CGname;
                saveObj.lastBG = GDB.BGName.None;
                CG.enabled = true;
                CG.sprite = handle.Result;
            }

            Addressables.Release(handle);
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
            lineNum++;
            CG.enabled = false;
        }
        yield return new WaitForSeconds(1f);
        Camera.SetBool("BlackOut", false);
        if (line.effects != GDB.Effects.BlackOut)
        {
            ApplyEffects(line.effects, true, line.time, line.V3position);
            yield return new WaitForSeconds(1.5f);
            ApplyEffects(line.effects, false, line.time, line.V3position);
        }
        CoroutinesWorking.Remove(cid);
        
    }
    public void decisionAction(string name)
    {
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
    }
    void menuAction(Item line)
    {
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
        int var = PlayerPrefs.GetInt(line.var.ToString());
        switch (line.signsIf)
        {
            case GDB.SignsIf.greater:
            {
               if (var > line.value)
                   jumpAction(line);
            }
                break;
            case GDB.SignsIf.less:
            {
                if (var < line.value)
                    jumpAction(line);
            }
                break;
            case GDB.SignsIf.equal:
            {
                if (var == line.value)
                    jumpAction(line);
            }
                break;
            default: Step(); break;
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
        lineNum++;
        Step();
        
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
    void textAction(Item line)
    {
        foreach (var actor in actorsOnScene)
        {
            actor.obj.GetComponentInChildren<SpriteRenderer>().color = UnityEngine.Color.gray;
        }
        GameObject ActorOnScene = FindActorOnScene(line.name.ToString());
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
           
           c = StartCoroutine(textCastEnum());
        }
        else
        {
            
            textContent.text = line.line;
        }
        textAuthor.text = line.name.ToString();
        lineNum++;
    }
    private IEnumerator textCastEnum()
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
