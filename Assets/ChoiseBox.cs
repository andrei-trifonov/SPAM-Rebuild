using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiseBox : MonoBehaviour
{
    public NewGameCore Core;
    public string jumpName;
    public TextMeshProUGUI TextMP;
    // Start is called before the first frame update


    public void MakeDecision()
    {
        Core.decisionAction(jumpName);
    }
    

    public void SetMenuItem(string text, string jumpName, NewGameCore Core)
    {
        TextMP.text = text;
        this.Core = Core;
        this.jumpName = jumpName;
    }
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
