using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
   public GameObject SaveGroup;

   public void RenewSaves()
   {
      foreach (LoadButton child in SaveGroup.transform.GetComponentsInChildren<LoadButton>())
      {
         child.Renew();
      }
   }
}
