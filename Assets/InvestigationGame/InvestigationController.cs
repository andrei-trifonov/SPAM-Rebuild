using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    [SerializeField] private string invName;
    [SerializeField] private Vector4 ScreenSpace;
    [SerializeField] List<ThoughtSt> Thoughts;
    [SerializeField] List<Result> Results;
    [SerializeField] List<MergeConnection> Merges;
    [SerializeField] GameObject thoughtTemplate;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultText;
     GameObject Brain;
    [SerializeField] Canvas canInv;
    [SerializeField] Canvas canTho;
    private Canvas canText;

    public GameObject cameraMain;
    GameObject cameraInvestigation;
     GameObject cameraThought;
    [SerializeField] GameObject Scene;
    private NewGameCore Core;
    private Result chosenResult;  List<GameObject> spawnedObjects = new List<GameObject>();
    // Start is called before the first frame update

    private void Start()
    {
        cameraMain = GameObject.FindGameObjectWithTag("MainCamera");
        canText = GameObject.Find("TextCanvas").GetComponent<Canvas>();
        cameraInvestigation = GameObject.FindGameObjectWithTag("InvCam");
        cameraThought = GameObject.FindGameObjectWithTag("ThoCam");
        Core = GameObject.FindObjectOfType<NewGameCore>();
        Brain = GameObject.FindGameObjectWithTag("Brain");
        cameraMain.SetActive(false);
        canText.enabled = false;
        string[] elements;
        if (PlayerPrefs.GetString(invName).Length > 0)
        {
            elements = PlayerPrefs.GetString(invName).Split("|");
            int counter = 0;
            ThoughtSt thought = new ThoughtSt();
            for (int i = 0; i < elements.Length; i++)
            {
               
                counter++;
                switch (counter)
                {
                    case 1:
                        thought.ID = int.Parse(elements[i]); break;
                    case 2: 
                        thought.Content = elements[i]; break;
                        
                    case 3:
                        {
                            
                            thought.Type = (ThoughtType)Enum.Parse(typeof(ThoughtType), elements[i]);
                            AddThought(thought);
                            thought = new ThoughtSt();
                            counter = 0;
                        } break;  
                }
            }
        }
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
        foreach (ThoughtSt item in Thoughts)
        {
            Thought spawned = Instantiate(thoughtTemplate, cameraThought.transform.position + new Vector3(UnityEngine.Random.Range( ScreenSpace.x, ScreenSpace.y),  UnityEngine.Random.Range(ScreenSpace.z, ScreenSpace.w), gameObject.transform.position.z), gameObject.transform.rotation).GetComponent<Thought>();
            spawned.transform.position = cameraThought.transform.position + new Vector3(UnityEngine.Random.Range(ScreenSpace.x, ScreenSpace.y), UnityEngine.Random.Range(ScreenSpace.z, ScreenSpace.w), 10);
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

    public void AddThought(ThoughtSt obj)
    {
        if (!Thoughts.Contains(obj))
        Thoughts.Add(obj);
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
        resultPanel.SetActive(false);
        PlayerPrefs.SetInt(chosenResult.Var.ToString(), 1);
        Core.jumpAction(chosenResult.jumpLabel);
        canText.enabled = true;
        cameraMain.SetActive(true);
        cameraThought.SetActive(true);
        cameraInvestigation.SetActive(true);
        Destroy(gameObject.transform.parent.gameObject);
        
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
