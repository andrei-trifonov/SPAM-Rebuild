
  
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
   [System.Serializable]
   public class DialogueSaveData
   {
       public LabelSampleWrapper[] Labels;
   }
   
   [System.Serializable]
   public class LabelSampleWrapper
   {
       public string name;
       public Item[] lines;
   }

   [ExecuteInEditMode]
   public class DialogueManager : MonoBehaviour
   {
        private string filePath = "C:\\SPAM_Restored\\Assets\\dialogue.json";
   
       public Dialogue dialogue; // This should be assigned in Unity Inspector
      
       public void SaveDialogueToFile()
       {
           DialogueSaveData saveData = new DialogueSaveData();
           saveData.Labels = new LabelSampleWrapper[dialogue.Labels.Count];
   
           for (int i = 0; i < dialogue.Labels.Count; i++)
           {
               saveData.Labels[i] = new LabelSampleWrapper()
               {
                   name = dialogue.Labels[i].name,
                   lines = dialogue.Labels[i].lines.ToArray()
               };
           }                           
   
           string json = JsonUtility.ToJson(saveData);
           File.WriteAllText(filePath, json);
       }
   
       public void LoadDialogueFromFile()
       {
           if (File.Exists(filePath))
           {
               string json = File.ReadAllText(filePath);
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
               Debug.LogError("Dialogue file not found at path: " + filePath);
           }
       }
   
       // Example usage
       /*void Start()
       {
           SaveDialogueToFile(); // Save the dialogue to a file
           LoadDialogueFromFile(); // Load the dialogue from the file
       }*/
   }