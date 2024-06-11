using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnClick : MonoBehaviour
{
   public Animator m_Anim;
   private void OnMouseDown()
   {
    
      m_Anim.SetBool("Play",  !m_Anim.GetBool("Play"));
   }
}
