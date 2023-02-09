using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LogComposer : MonoBehaviour
{
    public int maxLogSize;
    public List<GDB.Name> logName;
    public List<string> logLine;
    public GameObject Group;
    public GameObject Prefab;
    public List <LogElement> Logs;
    public GameObject Visual;

    public void RenewLog(GDB.Name name, string line)
    {
        logName.Add(name);
        logLine.Add(line);
        
        if (logLine.Count >= maxLogSize)
            logLine.Remove(logLine[0]);
    }
    
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
        for (int i = logLine.Count-1; i>= 0; i--)
        {
            LogElement inst = Instantiate(Prefab, Group.transform).GetComponent<LogElement>();
            inst.Name.text = logName[i].ToString();
            inst.Name.color = GDB.CharColor((int)logName[i]);
            inst.Text.text = logLine[i];
            Logs.Add(inst);
        }
    }
}
