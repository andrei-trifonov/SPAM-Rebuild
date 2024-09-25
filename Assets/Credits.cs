using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public struct CreditsItem 
{
    [SerializeField] public Sprite sprite;
   
    [SerializeField] public bool Title;
    [SerializeField] public string Text;
  
     [SerializeField] public string Text3;
}

public class Credits : MonoBehaviour
{

    [SerializeField] public List<CreditsItem> creditList;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject itemTemplate;
    [SerializeField] private float Speed;
    private Vector3 LastPos;
    private Vector3 StartPos;
    float t = 0;
    [SerializeField] private Transform Camera;
    // Start is called before the first frame update
    void Start()
    {
        StartPos = Camera.position;
        int shift = 0;
        for(int i=0; i< creditList.Count; i++)
        {
            CreditsItem item = creditList[i];
            GameObject IT = Instantiate(itemTemplate);
            if (item.sprite!= null)
                IT.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = item.sprite;
            if (item.Text!= null)
                IT.transform.GetChild(1).GetComponent<TMP_Text>().text = item.Text;
            if (item.Title)
                 IT.transform.GetChild(2).GetComponent<TMP_Text>().text = item.Text3;
            

            Vector3 Pos;
            if (IT.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite != null)
            {
                shift += 3;
                Pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + shift , 0);
                IT.transform.position = Pos;
                shift += 2;
            } else
            if (item.Text!=null)
            {
                shift += 1;
                Pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + shift , 0);
                IT.transform.position = Pos;
            }
            else if (item.Text3!=null)
            {
                shift += 2;
                Pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + shift , 0);
                IT.transform.position = Pos;
                shift += 2;
            }
            else
            {
                shift += 3;
                Pos = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y + shift , 0);
            }

            LastPos = new Vector3(StartPos.x ,Pos.y, StartPos.z);


        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (t <= 10)
        {
            t += Time.deltaTime/Speed;
        }

        Camera.position = Vector3.Lerp(StartPos, LastPos, t);
    }
}
