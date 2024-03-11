using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



[System.Serializable]
public class MergeConnection {
    public int Item1;
    [HideInInspector] public Thought t1;
    public int Item2;
    [HideInInspector] public Thought t2;
    public ThoughtSt Result;

}

[System.Serializable]
public class ThoughtSt
{
    
    public int ID;
    public string Content;
    public int Level;
    public ThoughtType Type;

    public bool Locked;
}

[System.Serializable]
public class UnlockMessage
{
    public UnlockMessage(string invName, int ID)
    {
        this.ID = ID;
        this.InvName = invName;
    }
    public int ID;
    public string InvName;

}


[System.Serializable]
public struct Result
{
    public int ID;
    public string Content;
    public GDB.Variables Var;
    public string jumpLabel;
    public bool isTrueResult;
}
public class InvestigationController : MonoBehaviour
{
    
    [SerializeField] private GameObject drugsButton1;
    [SerializeField] private GameObject drugsButton2;
    [SerializeField] private Vector4 ScreenSpace;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] GameObject thoughtTemplate;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] private  GameObject Brain;
    [SerializeField] Canvas canInv;
    [SerializeField] Canvas canTho;

    [SerializeField]  TMP_Text textDrugs1;
    [SerializeField]  TMP_Text textDrugs2;

    [SerializeField] private  GameObject cameraInvestigation;
    [SerializeField] private   GameObject cameraThought;
   
    [SerializeField] private NewGameCore Core;
    private Result chosenResult;
    List<GameObject> spawnedObjects = new List<GameObject>();


    List<ThoughtSt> Thoughts = new List<ThoughtSt>();
    List<Result> Results = new List<Result>();
    List<MergeConnection> Merges = new List<MergeConnection>();
    private int _drugsCount;
    private GameObject Scene;
    [SerializeField] private GameObject MergeEffect;
    [SerializeField] private GameObject DiffusionEffect;
    [SerializeField] private GameObject TrueFinEffect;
    [SerializeField] private GameObject FalseFinEffect;

    [SerializeField] private GameObject drugOverlay;
    private bool c;
    private bool usedDrug;
    private List<int> hintIDs = new List<int>();
    private bool State;
    public void SetNewGame(string sceneName, List<int> Unlocks, int drugsCount)
    {
        usedDrug = false;
        drugsButton1.SetActive(true);
        drugsButton2.SetActive(true);
        StartCoroutine(LoadInv(sceneName, Unlocks));
        _drugsCount = drugsCount;
        Debug.Log(_drugsCount);
        textDrugs1.text = "SPAM-V: " + drugsCount.ToString();
        textDrugs2.text = "SPAM-V: " + drugsCount.ToString();
        canInv.enabled = true;
    }

    IEnumerator LoadInv(string line, List<int> Unlocks)
    {


        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(line);
        yield return handle;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject res = handle.Result;
            Scene = Instantiate(res, spawnPoint.position, spawnPoint.rotation);

        }

        Addressables.Release(handle);

        InvestigationScenario scenario = Scene.GetComponent<InvestigationScenario>();
        Thoughts = scenario.Thoughts;
        Results = scenario.Results;
        Merges = scenario.Merges;
        if (Unlocks.Count > 0)
            try
            {
                foreach (int unlockID in Unlocks)
                {
                    foreach (var thought in Thoughts)
                    {
                        if (thought.ID == unlockID)
                            thought.Locked = false;
                    }

                }
            }
            catch
            {
            }
        

    }
    
   
    //TODO инкремент переменной

    public void UseDrugs()
    {
        if (_drugsCount > 0)
        {
            if (!State)  
                drugOverlay.SetActive(true);
            
            usedDrug = true;
            Core.UseDrugs();
 
            List<GameObject> interactiveObjects = new List<GameObject>();
            interactiveObjects = GameObject.FindGameObjectsWithTag("Interactive").ToList();

            foreach (var item in interactiveObjects)
            {
                item.GetComponent<MeshRenderer>().material.SetColor("_OutlineColor", UnityEngine.Color.yellow);
            }
            drugsButton1.SetActive(false);
            drugsButton2.SetActive(false);
        }

       
            foreach (var result in Results)
            {
                int r_id;
                if (result.isTrueResult)
                {
                    r_id = result.ID;
                    hintIDs.Add(r_id);
                    recMergeSearch(r_id);

                    break;
                }
            }
            if (State)
            {
            DestroyAllThought();
            SpawnThoughts();
        }
    }

    public void AddThought(ThoughtSt obj)
    {
        foreach (var item in Thoughts)
        {
            if (item.Content == obj.Content)
            {
                return;
            }
        }
      
        Thoughts.Add(obj);
            
    }

    void DestroyAllThought() {

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
               
        } spawnedObjects.Clear();
    }

    public void OpenThought()
    {
        State = true;
        if (usedDrug)
        {
            drugOverlay.SetActive(false);
        }
        DestroyAllThought();
        Scene.SetActive(false);
        cameraThought.SetActive(true);
        cameraInvestigation.SetActive(false);
        canTho.enabled = true;
        canInv.enabled = false;
        Brain.SetActive(true);
        SpawnThoughts();
     
    }

    void recMergeSearch(int id)
    {
        if (Merges.Count>0)
            foreach (var merge in Merges)
            {

                if (merge.Result.ID == id)
                {
                    hintIDs.Add(merge.Item1);
                    hintIDs.Add(merge.Item2);
                    recMergeSearch(merge.Item1);
                    recMergeSearch(merge.Item2);
                }
            }
    }
    private void SpawnThoughts()
    {
   
        foreach (ThoughtSt item in Thoughts)
        {
            if (!item.Locked)
            {
                Thought spawned = Instantiate(thoughtTemplate,
                    cameraThought.transform.position +
                    new Vector3(UnityEngine.Random.Range(ScreenSpace.x, ScreenSpace.y),
                        UnityEngine.Random.Range(ScreenSpace.z, ScreenSpace.w), gameObject.transform.position.z),
                    gameObject.transform.rotation).GetComponent<Thought>();
                spawned.transform.position = cameraThought.transform.position +
                                             new Vector3(UnityEngine.Random.Range(ScreenSpace.x, ScreenSpace.y),
                                                 UnityEngine.Random.Range(ScreenSpace.z, ScreenSpace.w), 10);
                spawnedObjects.Add(spawned.gameObject);
                if (hintIDs.Contains(item.ID))
                    spawned.Initiate(item.Content, item.Level, item.Type, item.ID, MergeEffect, true);
                else
                    spawned.Initiate(item.Content, item.Level, item.Type, item.ID, MergeEffect, false);
                foreach (MergeConnection connection in Merges)
                {
                    if (item.ID == connection.Item1)
                    {
                        connection.t1 = spawned.GetComponent<Thought>();
                    }

                    if (item.ID == connection.Item2)
                    {
                        connection.t2 = spawned.GetComponent<Thought>();
                    }

                }
            }
        }
    }
    
    public void CloseThought()
    {
        if (usedDrug)
        {
            drugOverlay.SetActive(true);
        }
        State = false;
        Scene.SetActive(true);
        DestroyAllThought();
        cameraThought.SetActive(false);
        canTho.enabled = false;
        canInv.enabled = true;
        cameraInvestigation.SetActive(true);
        Brain.SetActive(false);
    }

  

    public bool FinishInvestigation(int ID)
    {
       
        foreach(Result res in Results)
        {
            if (res.ID == ID)
            {
                StartCoroutine(FinishCoroutineT(res));
                return true;
            }
        }
        StartCoroutine(FinishCoroutineF());
        return false;
    }

    IEnumerator FinishCoroutineT(Result res)
    {
        TrueFinEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        TrueFinEffect.SetActive(false);
        DestroyAllThought();
        cameraThought.SetActive(false);
        cameraInvestigation.SetActive(true);
        canTho.enabled = false;
        canInv.enabled = true;
        resultPanel.SetActive(true); 
        resultText.text = res.Content;
        chosenResult = res;
    }
    IEnumerator FinishCoroutineF()
    {
        FalseFinEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        FalseFinEffect.SetActive(false);
        
    }
    public void EndGame()
    {
        Core.EndInv();
        
        resultPanel.SetActive(false);
        
        //TODO start
        PlayerPrefs.SetInt(chosenResult.Var.ToString(), 1);
        //TODO finish
        
        Core.jumpAction(chosenResult.jumpLabel);
       
        cameraThought.SetActive(true);
        cameraInvestigation.SetActive(true);
        canInv.enabled = false;
        Destroy(Scene);
        
        
    }

    IEnumerator DiffusionCoroutine()
    {
        if (!c)
        {
            c = true;
            DiffusionEffect.SetActive(true);
            yield return new WaitForSeconds(2);
            DiffusionEffect.SetActive(false);
            c = false;
        }
    }

    // Update is called once per frame
    public void UpdateCollisions(int ID)
    {
       
        try
        {
            foreach (MergeConnection connection in Merges)
            {
                if ((connection.t1.toMerge[0].ID == connection.Item2 || connection.t2.toMerge[0].ID == connection.Item1) && (connection.Item2 == ID || connection.Item1 == ID))
                {
                    
                    
                   
                    Thought spawned = Instantiate(thoughtTemplate).GetComponent<Thought>();
                    
                    if (hintIDs.Contains(connection.Result.ID))
                        spawned.Initiate(connection.Result.Content, connection.Result.Level, connection.Result.Type, connection.Result.ID, MergeEffect, true);
                    else
                        spawned.Initiate(connection.Result.Content, connection.Result.Level, connection.Result.Type, connection.Result.ID, MergeEffect, false);
                    
                    spawned.transform.position = new Vector3(connection.t1.gameObject.transform.position.x, connection.t1.gameObject.transform.position.y, connection.t1.gameObject.transform.position.z);
                    DiffusionEffect.transform.position = spawned.transform.position;
                    StartCoroutine(DiffusionCoroutine());
                    spawnedObjects.Add(spawned.gameObject);
                    spawnedObjects.Remove(connection.t1.gameObject);
                    spawnedObjects.Remove(connection.t2.gameObject);
                    Destroy(connection.t1.gameObject);
                    Destroy(connection.t2.gameObject);
                        
                    foreach (MergeConnection connection2 in Merges)
                    {
                        if (connection.Result.ID == connection2.Item1)
                        {
                            connection2.t1 = spawned.GetComponent<Thought>();
                        }
                        if (connection.Result.ID == connection.Item2)
                        {
                            connection2.t2 = spawned.GetComponent<Thought>();
                        }

                    }
                   
                    break;
                }
            }
        }
        catch { }
        }

    
}
