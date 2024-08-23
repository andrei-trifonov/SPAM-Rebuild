using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnClick : MonoBehaviour
{
   public GameObject Object;
   private void OnMouseDown()
   {
    
      Object.SetActive(true);
   }
}
