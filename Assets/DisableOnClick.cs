using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnClick : MonoBehaviour
{
   public GameObject m_obj;
   private void OnMouseDown()
   {
    
      m_obj.SetActive(false);
   }
}
