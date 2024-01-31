using System;
using System.Collections;
using System.Collections.Generic;
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

[System.Serializable]
public class m_Actor{
    public GameObject obj;
    public string name;
    public string pose;
}
public class NewGameCore : MonoBehaviour
{
    [SerializeField] private Canvas textCanvas;
    private Animator textCanvasAnimator;
    [SerializeField] private GameObject newThoughtLabel;
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
    
    [SerializeField] private SpriteRenderer CG;
    [SerializeField] private VideoPlayer BG;
    private VideoClip clipBG;

    private AudioSource fadeInMusic;
    private AudioSource fadeOutMusic;
    [SerializeField] GameObject musicPlayer;
    [SerializeField] private AudioSource sceneAudioPlayer;
    [SerializeField] private AudioSource soundPlayer;

    [SerializeField] private GameObject saveLoadMarker;

    private List<m_Actor> actorsOnScene = new List<m_Actor>();
    
   
    private bool loading;
    private bool hideText;
    
    string lastBG="";
    string lastMusic="";
    string lastCG = "";
    GDB.Name lastAuthor = GDB.Name.Мира;
    string lastLine = "";
   
    public List<int> CoroutinesWorking;
    int cid;

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
        textCanvasAnimator = textCanvas.GetComponent<Animator>();
        scenario = scenarioComposer.Labels;
    }
    public void ClearSprites()
    {
        foreach (m_Actor obj in actorsOnScene)
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

            //Load music
            Item item = new Item();
            if (PlayerPrefs.GetString(savenum + "Music").Length > 0)
                item.music = (GDB.Music)Enum.Parse(typeof(GDB.Music), PlayerPrefs.GetString(savenum + "Music"));
                musicAction(item);

            //Get 
            string[] elements = { };
            if (PlayerPrefs.GetString(savenum + "Actors").Length > 0) {
                elements = PlayerPrefs.GetString(savenum + "Actors").Split("|");
                int counter = 0;

                for (int i = 0; i < elements.Length; i++)
                {
                   // Debug.Log(elements[i]);
                    counter++;
                    switch (counter)
                    {
                        case 1:
                            item.name = (GDB.Name)Enum.Parse(typeof(GDB.Name), elements[i]); break;
                        case 2:
                            item.V3position.x = int.Parse(elements[i]); break;
                        case 3:
                            item.V3position.y = int.Parse(elements[i]); break;
                        case 4:
                            item.V3position.z = int.Parse(elements[i]); break;
                        case 5:
                            {
                                item.pose = (GDB.Pose)Enum.Parse(typeof(GDB.Pose), elements[i]);
                                actorAction(item);
                                item = new Item();
                                counter = 0;
                            }
                            break;
                    }

                }
            }
           

            //Return variables
            item = new Item();

            if (PlayerPrefs.GetString(savenum + "Variables").Length > 0)
            {
                //Debug.Log(PlayerPrefs.GetString(savenum + "Variables"));
                elements = PlayerPrefs.GetString(savenum + "Variables").Split("|");
                int counter = 0;
                //Debug.Log(elements[0]);

                for (int i = 0; i < elements.Length; i++)
                {
                   // Debug.Log(elements[i]);
                    counter++;
                    switch (counter)
                    {
                        case 1:
                            item.var = (GDB.Variables)Enum.Parse(typeof(GDB.Variables), elements[i]); break;
                        case 2:
                            { 
                                item.value = int.Parse(elements[i]); 
                                item.signs = GDB.Signs.equal;
                                varAction(item);
                                item = new Item();
                                counter = 0;
                            } break;
                    }

                }
            }

            //Spawn Scene
            item = new Item();
            if ( PlayerPrefs.GetString(savenum + "Scene").Length>0)
                item.BGname = (GDB.BGName)Enum.Parse(typeof(GDB.BGName), PlayerPrefs.GetString(savenum + "Scene"));
            bgAction(item);
            //Return line num label num
            lineNum = PlayerPrefs.GetInt(savenum + "Line");
            labelNum = PlayerPrefs.GetInt(savenum + "Label");


            //return text to panel
            GDB.Name.TryParse(PlayerPrefs.GetString(savenum + "AuthorText"), out lastAuthor);
            lastLine = PlayerPrefs.GetString(savenum + "LineText");
            textContent.text = lastLine;
            textAuthor.color = GDB.CharColor((int)lastAuthor);
            textAuthor.text = lastAuthor.ToString();
            Debug.Log(PlayerPrefs.GetInt(("APoints")));
            Debug.Log(PlayerPrefs.GetInt(("MPoints")));
            BlinkSLMarker(1);
            loading = false;
          
        }
    }
    public void Save(string savenum)
    {
        if (!loading && CoroutinesWorking.Count == 0)
        {
            PlayerPrefs.SetString("LastSave", savenum);
            string save = "";
            foreach (m_Actor actor in actorsOnScene)
            {
                save = save + actor.name + "|" + actor.obj.transform.position.x + "|" + actor.obj.transform.position.y + "|" + actor.obj.transform.position.z + "|" + actor.pose + "|";

            }
            if (save.Length>0) { save = save.Substring(0, save.Length - 1); } 
           
            Debug.Log("Actors "  + save);


            PlayerPrefs.SetString(savenum + "AuthorText", lastAuthor.ToString());
            Debug.Log("AuthorText "  + lastAuthor);
            
            PlayerPrefs.SetString(savenum + "LineText", lastLine);
            Debug.Log("LineText "  + lastLine);
            
            PlayerPrefs.SetString(savenum + "Actors", save);

            PlayerPrefs.SetString(savenum + "Scene", lastBG);
            Debug.Log("Scene "  + lastBG);

            PlayerPrefs.SetString(savenum + "CG", lastCG);
            Debug.Log("CG "  + lastBG);
            
            PlayerPrefs.SetString(savenum + "Music", lastMusic);
            Debug.Log("Music "  + lastMusic);

            PlayerPrefs.SetInt(savenum + "isSaved", 1);

            PlayerPrefs.SetInt(savenum + "Line", lineNum);
            Debug.Log("Line "  + lineNum);
            
            PlayerPrefs.SetInt(savenum + "Label", labelNum);
            Debug.Log("Label "  + labelNum);
            
            Enum tmpEnum = new GDB.Variables();
            save = "";
            foreach (string element in Enum.GetNames(tmpEnum.GetType()))
            {
                save = save + element + "|" + PlayerPrefs.GetInt(element) + "|";
            }
            if (save.Length > 0)
            {
                save = save.Substring(0, save.Length - 1);
            }
           // Debug.Log(save);

            PlayerPrefs.SetString(savenum + "Variables", save);

            PlayerPrefs.SetString(savenum + "Savetime", "" + System.DateTime.Now);
            

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
            case GDB.LineType.Investigation:
                invAction(line);
                break;
        }
    }
    void invAction(Item line)
    {
        if (line.inv == GDB.Investigation.Open)
             StartCoroutine(LoadInv(line, cid++));
        else
        {
            StartCoroutine(NewThoughtCoroutine());
            PlayerPrefs.SetString(line.investigationName, (line.value + "|" + line.additionalPose + "|" + line.ThoType.ToString()));
            lineNum++;
            Step();
        }
    }
    IEnumerator NewThoughtCoroutine()
    {
        newThoughtLabel.SetActive(true);
        yield return new WaitForSeconds(2);
        newThoughtLabel.SetActive(false);
    }
    IEnumerator LoadInv(Item line, int id)
    {
        CoroutinesWorking.Add(id);
      
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(line.additionalPose);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject res = handle.Result;
            Instantiate(res);
        }
        Addressables.Release(handle);
        CoroutinesWorking.Remove(id);


    }
    void cameraAction(Item line)
    {

        StartCoroutine(EffectCoroutine(line, cid++));


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
        foreach(m_Actor obj in actorsOnScene)
        {
            if (obj.name == name)
            {
                return obj.obj;
            }
        }

        return null;
    }
    m_Actor FindActorOnScenePointer(string name)
    {
        foreach (m_Actor obj in actorsOnScene)
        {
            if (obj.name == name)
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
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(line.name.ToString());
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject res = handle.Result;
                ActorOnScene = GameObject.Instantiate(res);
                m_Actor actor = new m_Actor();
                actor.name = line.name.ToString();
                actor.obj = ActorOnScene;
                actor.pose = line.pose.ToString();
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
            FindActorOnScenePointer(line.name.ToString()).pose = line.pose.ToString();
        }


        lineNum++;
        CoroutinesWorking.Remove(id);
        

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
        lastMusic = line.music.ToString();
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
        lastBG = line.BGname.ToString();
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
                lastCG = line.CGname;
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
            lastCG = "";
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
            ChoiseBox cb;
            cb = Instantiate(choiseBox, choiseGroup.transform).GetComponent<ChoiseBox>();
            cb.SetMenuItem(choiseVariant, line.menu_jump[line.menu_label.IndexOf(choiseVariant)], this);
            choiseBoxes.Add(cb.gameObject);
        }
    }

    IEnumerator MenuActionCoroutine()
    {
        choiseRoulette = true;
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
        //Debug.Log(line.value); 
        switch (line.signs)
        {
            case GDB.Signs.decr: 
                PlayerPrefs.SetInt(line.var.ToString(), PlayerPrefs.GetInt(line.var.ToString())-1);
                break;
            case GDB.Signs.incr: 
                PlayerPrefs.SetInt(line.var.ToString(), PlayerPrefs.GetInt(line.var.ToString())+1);
                break;
            case GDB.Signs.equal: 
                PlayerPrefs.SetInt(line.var.ToString(), line.value);
                break;
        }
        lineNum++;
        Step();
        
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
        ApplyTextEffects(GDB.Fonts.Regular);
        lastAuthor = line.name;
        lastLine = line.line;
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
