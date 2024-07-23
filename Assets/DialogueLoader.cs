using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class DialogueLoader : MonoBehaviour
{
  private string filePath = "C:\\SPAM_Restored\\Assets\\dialogue";
   
       public Dialogue dialogue; // This should be assigned in Unity Inspector


[ExecuteInEditMode]
public void LoadDialogueFromFile()
       {
           if (File.Exists(filePath +gameObject.name + ".json"))
           {
               string json = File.ReadAllText(filePath + gameObject.name + ".json");
               DialogueSaveData saveData = JsonUtility.FromJson<DialogueSaveData>(json);

   
               // Reconstruct the dialogue object from the loaded data
            
               dialogue.Labels = new List<LabelSample>();
   
               foreach (LabelSampleWrapper labelWrapper in saveData.Labels)
               {
                   LabelSample label = new LabelSample
                   {
                       name = labelWrapper.name,
                       lines = new List<Item>(labelWrapper.lines)
                   };
                   dialogue.Labels.Add(label);
               }
           }
           else
           {
               Debug.LogError("Dialogue file not found at path: " + gameObject.name  + ".json");
           }
       }
}
