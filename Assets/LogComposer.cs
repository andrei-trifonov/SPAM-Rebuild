using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        string[] words = line.Split(' ');
        List<string> lines = new List<string>();
        string currentLine = "";
        foreach (string word in words)
        {
            if (currentLine.Length + word.Length > 230)
            {
                lines.Add(currentLine.Trim());
                currentLine = word;
            }
            else
            {
                currentLine += " " + word;
            }
        }
        lines.Add(currentLine.Trim());
        foreach (string line_ in lines)
        {
            logName.Add(name);
            logLine.Add(line_);
            
            if (logLine.Count >= maxLogSize){
                logLine.Remove(logLine[0]);
                logName.Remove(logName[0]);
            }
        }
       
        
       
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
            inst.Name.text = logName[i].ToString().Replace("_", "-");
            inst.Name.color = GDB.CharColor((int)logName[i]);
            inst.Text.text = logLine[i];
            Debug.Log(logName[i]);
            Debug.Log(i);
            Debug.Log(logLine[i]);
            Logs.Add(inst);
        }
    }
}
