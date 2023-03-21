using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
[System.Serializable]
public class m_Actor{
    public GameObject obj;
    public string name;
    public string pose;
}
public class NewGameCore : MonoBehaviour
{
    [SerializeField] private Canvas textCanvas;
    
    [SerializeField] private TextMeshProUGUI textAuthor;
    [SerializeField] private TextMeshProUGUI textContent;
    
    [SerializeField] private Dialogue scenarioComposer;
    private List<LabelSample> scenario;

    private float maxVolumeMusic = 1; //TODO
    private Coroutine c;
    private float textDelay = 0.1f; //TODO 
    private bool skipping; //TODO
    
    private int labelNum;
    private int lineNum;
 
    private string castingLine;
    private bool loadC;
    [SerializeField] private LogComposer Log;
    
    public List<GameObject> choiseBoxes;
    private GameObject choiseBox;
    [SerializeField] private LayoutGroup choiseGroup;
    [SerializeField] private Image CG;
    private GameObject BG;

    private AudioSource fadeInMusic;
    private AudioSource fadeOutMusic;
    [SerializeField] GameObject musicPlayer;

    [SerializeField] private AudioSource soundPlayer;
   
    
    private List<m_Actor> actorsOnScene = new List<m_Actor>();
    
    private bool blocked;
    private bool loading;

    string lastBG="";
    string lastMusic="";
    string lastCG = "";
    GDB.Name lastAuthor = GDB.Name.Мира;
    string lastLine = "";
   
    public List<int> CoroutinesWorking;
    int cid;

    private void Start()
    {
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
    public void Load(string savenum)
    {
        if (!loading && CoroutinesWorking.Count == 0)
        {
            StopAllCoroutines();
            loading = true;
            skipping = false;

            ClearSprites();
            CG.enabled = false;
            try { Destroy(BG); }
            catch { }



            Item item = new Item();

            musicAction(item);

            string[] elements = PlayerPrefs.GetString(savenum + "Actors").Split("|");
            int counter = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                Debug.Log(elements[i]);
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

            elements = PlayerPrefs.GetString(savenum + "Variables").Split("|");
            for (int i = 0; i < elements.Length; i++)
            {
                Debug.Log(elements[i]);
            }


            lineNum = PlayerPrefs.GetInt(savenum + "Line");
            labelNum = PlayerPrefs.GetInt(savenum + "Label");

            textContent.text = lastLine;

            textAuthor.color = GDB.CharColor((int)lastAuthor);
            textAuthor.text = lastAuthor.ToString();

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
            save = save.Substring(0, save.Length - 1);
            Debug.Log(save);


            PlayerPrefs.SetString(savenum + "Actors", save);

            PlayerPrefs.SetString(savenum + "Scene", lastBG);

            PlayerPrefs.SetString(savenum + "CG", lastCG);

            PlayerPrefs.SetString(savenum + "Music", lastMusic);

            PlayerPrefs.SetInt(savenum + "isSaved", 1);

            PlayerPrefs.SetInt(savenum + "Line", lineNum);

            PlayerPrefs.SetInt(savenum + "Label", labelNum);

            Enum tmpEnum = new GDB.Variables();
            save = "";
            foreach (string element in Enum.GetNames(tmpEnum.GetType()))
            {
                save = save + element + "|" + PlayerPrefs.GetInt(element) + "|";
            }
            save = save.Substring(0, save.Length - 1);
            Debug.Log(save);

            PlayerPrefs.SetString(savenum + "Variables", save);

            PlayerPrefs.SetString(savenum + "Savetime", "" + System.DateTime.Now);

            PlayerPrefs.Save();
        }
    }


    public void SkipDown()
    {
        skipping = true;
        StartCoroutine(SkipCoroutine());
    }


    IEnumerator SkipCoroutine()
    {
        while (skipping && !blocked)
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
        if (!blocked && !loadC && !loading && CoroutinesWorking.Count==0)
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
        }
    }
    
    void actorAction(Item line)
    {
        //if (loadC == false)
        //{
           StartCoroutine(LoadActor(line, cid++));
           
            
    
        //}

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
        //TODO Effects

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
        
        if(line.show)
            StartCoroutine(LoadSound(line, cid++));
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
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(line.additionalPose);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip res = handle.Result;
            soundPlayer.PlayOneShot(res);
            lineNum++;
            CoroutinesWorking.Remove(cid);
            Step();
        }

        Addressables.Release(handle);
    }
    void musicAction(Item line)
    {
       if(line.show)
          StartCoroutine(LoadMusic(line, cid++));
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
            CoroutinesWorking.Remove(cid);
            Step();
        }

        Addressables.Release(handle);
    }
    void bgAction(Item line)
    {
        if (loadC == false)
        {
            StartCoroutine(LoadBackground(line));
        }
    }

    IEnumerator LoadBackground(Item line)
    {
        CoroutinesWorking.Add(cid);
        lastBG = line.BGname.ToString();
        loadC = true;
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(line.BGname.ToString());
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            try { Destroy(BG); } catch (Exception e) { 
            }
            GameObject res = handle.Result;
            BG = Instantiate(res);
            lineNum++;
            loadC = false;
            CoroutinesWorking.Remove(cid);
        }
       
        Addressables.Release(handle);
        //TODO Effects
        
    }
    
    void cgAction(Item line)
    {
     if(loadC == false)
        StartCoroutine(LoadSprite(line));
     

     //TODO Эффекты 
    }

    IEnumerator LoadSprite(Item line)
    {
         CoroutinesWorking.Add(cid);
        if (line.show)
        {
            //TODO ЭФфекты
            loadC = true;
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
            loadC = false;
        }
        else
        {
            if (!skipping)
            {
                blocked = true;
                //TODO ЭФфекты
                yield return new WaitForSeconds(1);
                blocked = false;
                
            }
            lastCG = "";
            lineNum++;
            CG.enabled = false;
        }
        CoroutinesWorking.Remove(cid);
        
    }
    public void decisionAction(string name)
    {
        
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
    void menuAction(Item line)
    {
        textCanvas.enabled = false;
        foreach (var choiseVariant in line.menu_label)
        {
            ChoiseBox cb;
            cb = Instantiate(choiseBox, choiseGroup.transform).GetComponent<ChoiseBox>();
            cb.SetMenuItem(choiseVariant, line.menu_jump[line.menu_label.IndexOf(choiseVariant)], this);
            choiseBoxes.Add(cb.gameObject);
        }
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
        Step();
    }

    void textAction(Item line)
    {
        lastAuthor = line.name;
        lastLine = line.line;
        if(c!= null)
            StopCoroutine(c); 
        castingLine = "";
        textContent.text = "";

        textAuthor.color = GDB.CharColor((int)line.name);
       
        Log.RenewLog(line.name, line.line);

        switch (line.font)
        {
            //TODO Эффекты текста
        }
        
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
        foreach (var _char in castingLine)
        {
            textContent.text += _char;
            yield return new WaitForSeconds(textDelay);
        }
        
    }
}
