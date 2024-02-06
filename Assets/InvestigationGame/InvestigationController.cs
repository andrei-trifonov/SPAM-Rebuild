using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
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
}
public class InvestigationController : MonoBehaviour
{
    
  
    [SerializeField] private Vector4 ScreenSpace;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] GameObject thoughtTemplate;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] private  GameObject Brain;
    [SerializeField] Canvas canInv;
    [SerializeField] Canvas canTho;

    [SerializeField]  TMP_Text textDrugs;


    [SerializeField] private  GameObject cameraInvestigation;
    [SerializeField] private   GameObject cameraThought;
   
    [SerializeField] private NewGameCore Core;
    private Result chosenResult;
    List<GameObject> spawnedObjects = new List<GameObject>();


    List<ThoughtSt> Thoughts = new List<ThoughtSt>();
    List<Result> Results = new List<Result>();
    List<MergeConnection> Merges = new List<MergeConnection>();
    private int drugsCount;
    private GameObject Scene;
    public void SetNewGame(string sceneName, List<int> Unlocks, int drugsCount)
    {
  
        StartCoroutine(LoadInv(sceneName, Unlocks));
        this.drugsCount = drugsCount;
        textDrugs.text = "SPAM-V: " + drugsCount;
        canInv.enabled=true;
    
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

    public void AddThought(ThoughtSt obj)
    {
        if (!Thoughts.Contains(obj))
            Thoughts.Add(obj);
    }

    void DestroyAllThought() {

        for (int i = 0; i < spawnedObjects.Count; i++)
        {
            Destroy(spawnedObjects[i]);
               
        } spawnedObjects.Clear();
    }

    public void OpenThought() {

        DestroyAllThought();
        Scene.SetActive(false);
        cameraThought.SetActive(true);
        cameraInvestigation.SetActive(false);
        canTho.enabled = true;
        canInv.enabled = false;
        Brain.SetActive(true);
        SpawnThoughts();
     
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
                spawned.Initiate(item.Content, item.Level, item.Type, item.ID);
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
        Scene.SetActive(true);
        DestroyAllThought();
        cameraThought.SetActive(false);
        canTho.enabled = false;
        canInv.enabled = true;
        cameraInvestigation.SetActive(true);
        Brain.SetActive(false);
    }

  

    public void FinishInvestigation(int ID)
    {
       
        foreach(Result res in Results)
        {
            if (res.ID == ID)
            {

                DestroyAllThought();
                cameraThought.SetActive(false);
                cameraInvestigation.SetActive(true);
                canTho.enabled = false;
                canInv.enabled = true;
                resultPanel.SetActive(true); 
                resultText.text = res.Content;
                chosenResult = res;
               

              
            }
        }
       
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
                    spawned.Initiate(connection.Result.Content, connection.Result.Level, connection.Result.Type, connection.Result.ID);
                    spawned.transform.position = new Vector3(connection.t1.gameObject.transform.position.x, connection.t1.gameObject.transform.position.y, connection.t1.gameObject.transform.position.z);
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
