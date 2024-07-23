
  
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
   public class DialogueSaver : MonoBehaviour
   {
        private string filePath = "C:\\SPAM_Restored\\Assets\\dialogue";
   
       public List <Dialogue> Dialogues; // This should be assigned in Unity Inspector
      
       public void SaveDialogueToFile()
       {
	   
           Debug.Log("Запись");
           
          
           foreach(Dialogue dialogue in Dialogues){
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
          
           File.WriteAllText(filePath + dialogue.gameObject.name + ".json", json);
           
  	 }
    }
   
      
   
       // Example usage
       /*void Start()
       {
           SaveDialogueToFile(); // Save the dialogue to a file
           LoadDialogueFromFile(); // Load the dialogue from the file
       }*/
   }