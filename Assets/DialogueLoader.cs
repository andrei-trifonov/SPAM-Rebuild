using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


public class DialogueLoader : MonoBehaviour
{
  private string filePath =   Path.Combine("C:\\Users\\user2\\Documents\\SPAM_Restored\\Assets\\", "Dialogue\\");
   
       public Dialogue dialogue; // This should be assigned in Unity Inspector


[ExecuteInEditMode]
public void LoadDialogueFromFile()
{
          filePath = Path.Combine("C:\\Users\\user2\\Documents\\SPAM_Restored\\Assets\\", "Dialogue\\");
           Debug.Log(filePath +gameObject.name + ".json");
           if (File.Exists(filePath +gameObject.name + ".json"))
           {
               Debug.Log("OK");
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
               Debug.LogError("Dialogue file not found ");
           }
       }
}
