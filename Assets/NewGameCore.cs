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

    private bool blocked;

    private void Start()
    {
        scenario = scenarioComposer.Labels;
    }



    public void SkipDown()
    {
        skipping = true;
        StartCoroutine(SkipCoroutine());
    }

    IEnumerator SkipCoroutine()
    {
        while (skipping)
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
        if (!blocked)
        {
            if (labelNum == scenario.Count)
            {
                Debug.Log("Стоп");
                //TODO Конец игры
            }
            else
            {
                if (lineNum == scenario[labelNum].lines.Count)
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
        }
    }

    void pauseAction(Item line)
    {
        StartCoroutine (PauseCoroutine(line));
    }

    IEnumerator PauseCoroutine(Item line)
    {
        if (!blocked && !skipping)
        {
            blocked = true;
            yield return new WaitForSeconds(line.time);
            blocked = false;
            lineNum++;
            Step();
        }
        else
        {
            lineNum++;
            Step();
        }
    }
    void soundAction(Item line)
    {
        
        if(line.show)
            StartCoroutine(LoadSound(line));
        else
        {
            soundPlayer.Stop();
            lineNum++;
            Step();
        }
    
    }
    IEnumerator LoadSound(Item line)
    {
        loadC = true;
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(line.additionalPose);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip res = handle.Result;
            soundPlayer.PlayOneShot(res);
            loadC = false;
            lineNum++;
            Step();
        }

        Addressables.Release(handle);
    }
    void musicAction(Item line)
    {
       if(line.show)
          StartCoroutine(LoadMusic(line));
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
    IEnumerator LoadMusic(Item line)
    {
        if (fadeInMusic)
            fadeOutMusic = fadeInMusic;
        fadeInMusic = Instantiate(musicPlayer, transform).GetComponent<AudioSource>();
        loadC = true;
        AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(line.music.ToString());
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AudioClip res = handle.Result;
            fadeInMusic.clip = res;
            loadC = false;
            fadeInMusic.Play();
            fadeInMusic.GetComponent<Fader>().maxVolumeMusic = maxVolumeMusic;
            if (fadeOutMusic)
                fadeOutMusic.GetComponent<Fader>().fadingOut = true;
            fadeInMusic.GetComponent<Fader>().fadingIn = true;
            lineNum++;
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
        loadC = true;
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(line.CGname);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                CG.sprite = handle.Result;
            }
       
            Addressables.Release(handle);
            lineNum++;
        loadC = false;
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
