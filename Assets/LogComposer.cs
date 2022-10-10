using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LogComposer : MonoBehaviour
{
    public Core Engine;
    public List<string> logName;
    public List<string> logLine;
    public List<Color> logColor;
    public GameObject Group;
    public GameObject Prefab;
    public List <LogElement> Logs;
    public GameObject Visual;
    void Flush()
    {
        
        foreach (var le in Logs)
        {
            Destroy(le.gameObject);
        }
        Logs.Clear();
    }

    // Start is called before the first frame update
    public void Compose()
    {
        Visual.SetActive(true);
        Flush();
        logLine = Engine.logLine;
        logName = Engine.logName;
        logColor = Engine.logColor;
        for (int i = logLine.Count-1; i>= 0; i--)
        {
            LogElement inst = Instantiate(Prefab, Group.transform).GetComponent<LogElement>();
            inst.Name.text = logName[i];
            inst.Name.color = logColor[i];
            inst.Text.text = logLine[i];
            Logs.Add(inst);
        }
    }
}
