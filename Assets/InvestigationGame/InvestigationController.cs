using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;



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
    public string Content_EN;
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
    public string Content_EN;
    public GDB.Variables Var;
    public int Value;
    public string jumpLabel;
    public bool isTrueResult;
}
public class InvestigationController : MonoBehaviour
{

    [SerializeField] private GameObject sliderFromMainGame;
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
    List<GameObject> interactiveObjects = new List<GameObject>();
    private bool FirstTime = true;
    public bool inAction;
    private int curLocalization;
    public void SetNewGame(string sceneName, List<int> Unlocks, int drugsCount)
    {
        
        curLocalization = Core.GetLocalization();
        sliderFromMainGame.SetActive(false);
        usedDrug = false;
        drugsButton1.SetActive(true);
        drugsButton2.SetActive(true);
        StartCoroutine(LoadInv(sceneName, Unlocks));
        _drugsCount = drugsCount;
        Debug.Log(_drugsCount);
        textDrugs1.text = "SPAM: " + drugsCount.ToString();
        textDrugs2.text = "SPAM: " + drugsCount.ToString();
        canInv.enabled = true;
        if (Core.RetInjure())
        {
            drugsButton1.SetActive(false);
            drugsButton2.SetActive(false);
        }
    }

    IEnumerator LoadInv(string line, List<int> Unlocks)
    {


        
        ResourceRequest request = Resources.LoadAsync<GameObject>("Investigations/"+line);
                   
        while (!request.isDone)
        {
            yield return null;
        }

        if (request.asset == null)
        {
            Debug.LogError("Failed to load evevnt at path:Investigations/"+line);
        }
        else
        {
            GameObject obj = request.asset as GameObject;
            // Делаем что-то с загруженным спрайтом
           
       
            Scene = Instantiate(obj, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Inv loaded successfully!");
        }
        
       
        interactiveObjects = GameObject.FindGameObjectsWithTag("Interactive").ToList();

        InvestigationScenario scenario = Scene.GetComponent<InvestigationScenario>();
        Thoughts = scenario.Thoughts;
        Results = scenario.Results;
        Merges = scenario.Merges;
        foreach (var thought in Thoughts)
        {
            thought.Content = curLocalization==0?thought.Content:thought.Content_EN;
        }
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
            foreach (var item in interactiveObjects)
            {
                if (item.GetComponentInChildren<MeshRenderer>())
                    foreach (Material _mat in item.GetComponentInChildren<MeshRenderer>().materials)
                        try
                        {
                            _mat.SetColor("_OutlineColor", UnityEngine.Color.yellow);

                        }
                        catch
                        {
                        }


                else if (item.GetComponent<SpriteRenderer>())
                {
                    Material _mat_s = item.GetComponent<SpriteRenderer>().material;
                    if (_mat_s.GetColor("_Color") != Color.black)
                       _mat_s.SetColor("_OutlineColor", UnityEngine.Color.yellow);

                }
        



        }
            drugsButton1.SetActive(false);
            drugsButton2.SetActive(false);
        

       
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
            SpawnAllThoughts();
        }
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

        SpawnOneThought(obj);
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
        cameraThought.SetActive(true);
        State = true;
        if (usedDrug)
        {
            drugOverlay.SetActive(false);
        }
        if (FirstTime)
             DestroyAllThought();
        Scene.SetActive(false);
       
        cameraInvestigation.SetActive(false);
        canTho.enabled = true;
        canInv.enabled = false;
        Brain.SetActive(true);
        if (FirstTime)
            SpawnAllThoughts();
     
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
    private void SpawnAllThoughts()
    {

        FirstTime = false;
        foreach (ThoughtSt item in Thoughts)
        {

            SpawnOneThought(item);
        }
    }
      private void SpawnOneThought(ThoughtSt item )
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
                    spawned.gameObject.GetComponent<DragNDropObject>().ThoCam = cameraThought.GetComponent<Camera>();
                    spawned.gameObject.GetComponent<DragNDropObject>().IC = this;
                    if (hintIDs.Contains(item.ID))
                        spawned.Initiate(curLocalization==0?item.Content:item.Content_EN, item.Level, item.Type, item.ID, MergeEffect, true);
                    else
                        spawned.Initiate(curLocalization==0?item.Content:item.Content_EN, item.Level, item.Type, item.ID, MergeEffect, false);
                   
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
        //DestroyAllThought();
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
        Brain.SetActive(false);
        State = false;
        TrueFinEffect.SetActive(true);
        yield return new WaitForSeconds(1);
        TrueFinEffect.SetActive(false);
        DestroyAllThought();
        cameraThought.SetActive(false);
        cameraInvestigation.SetActive(true);
        canTho.enabled = false;
        canInv.enabled = true;
        resultPanel.SetActive(true); 
        resultText.text = curLocalization==0?res.Content:res.Content_EN;
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
        
        
        Core.UpdateVar(chosenResult.Var, chosenResult.Value);
        
        
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

    public void ResetProgress()
    {
        DestroyAllThought();
        SpawnAllThoughts();
    }
    
   Thought FindThought(int id){
    
    foreach (GameObject obj in spawnedObjects){
    
    if (obj.GetComponent<Thought>().ID == id)
    return obj.GetComponent<Thought>();
    }
    return new Thought();
    }
    // Update is called once per frame
    public void UpdateCollisions(int myID, int otherID)
    {


        
            foreach (MergeConnection connection in Merges)
            {
                if (connection.Item1 == myID && connection.Item2 == otherID ||
                    connection.Item1 == otherID && connection.Item2 == myID)
                {
                    Debug.Log("Pair found" + myID + " " + otherID);

                    
                    connection.t1 = FindThought(myID);
                    connection.t2 = FindThought(otherID);
                    Thought spawned = Instantiate(thoughtTemplate).GetComponent<Thought>();

                    if (hintIDs.Contains(connection.Result.ID))
                        spawned.Initiate(curLocalization==0?connection.Result.Content:connection.Result.Content_EN, connection.Result.Level, connection.Result.Type,
                            connection.Result.ID, MergeEffect, true);
                    else
                        spawned.Initiate(curLocalization==0?connection.Result.Content:connection.Result.Content_EN, connection.Result.Level, connection.Result.Type,
                            connection.Result.ID, MergeEffect, false);

                    spawned.transform.position = new Vector3(connection.t1.gameObject.transform.position.x,
                        connection.t1.gameObject.transform.position.y, connection.t1.gameObject.transform.position.z);
                   
                   
                    Destroy(connection.t1.gameObject);
                    Destroy(connection.t2.gameObject);
                    spawnedObjects.Remove(connection.t1.gameObject);
                    spawnedObjects.Remove(connection.t2.gameObject);
                    
                    DiffusionEffect.transform.position = spawned.transform.position;
                    StartCoroutine(DiffusionCoroutine());
                    spawnedObjects.Add(spawned.gameObject);
                  
                    spawned.gameObject.GetComponent<DragNDropObject>().ThoCam = cameraThought.GetComponent<Camera>();
                    spawned.gameObject.GetComponent<DragNDropObject>().IC = this;
                    
                    
                   

                    break;
                }
            }
        
      
        }

    
}
