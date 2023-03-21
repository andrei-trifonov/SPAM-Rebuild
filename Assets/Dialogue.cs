using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
   
    public List<LabelSample> Labels;
}

[System.Serializable]
public class Item
{
    public  GDB.LineType type;
    public  GDB.Name name;
    [Multiline] public string line;
    public GDB.Fonts font;
    
    
    public GDB.BGName BGname;
    public GDB.Effects effects;


    public string CGname;

    public bool show;
    public GDB.Pose pose;
    public string additionalPose;
    
    public GDB.Music  music;
    
    public float time;
    
    public int value;
    public GDB.Signs signs;
    public GDB.Variables var;

    public GDB.SignsIf signsIf;

    public List<string> menu_label;
    public List<string> menu_jump;
    public Vector3 V3position;
}

[System.Serializable]
public class LabelSample
{
    public string name;
    public List<Item> lines;

    public void AddLine()
    {
        lines.Add(new Item());
    }

}
   


