using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Choose : MonoBehaviour
{
    public GameObject Core;
    public int Lookup;
    public int Return;
    public TextMeshProUGUI TextMP;
    // Start is called before the first frame update
    public void SetLookup(int i)
    {
        Lookup = i;
    }

    public void MakeDecision()
    {
        gameObject.transform.parent.GetComponent<GroupManager>().Clear();
        Core.GetComponent<Core>().ChooseConseq(Lookup);
        Core.GetComponent<Core>().ChooseReturn(Return);
        Core.GetComponent<Core>().SetBlock(false);
        Core.GetComponent<Core>().Step();
    }

    public void SetReturnPoint(int line)
    {
        Return = line;
    }

    public void SetParent(GameObject go)
    {
        Core = go;
    }

    public void SetText(string text)
    {
        TextMP.text = text;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
