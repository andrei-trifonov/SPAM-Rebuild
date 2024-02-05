using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObject : MonoBehaviour
{
    public SaveObject()
    {
        unlockedInv = new List<UnlockMessage>();
    }
    public List<UnlockMessage> unlockedInv;
    
}
