using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesnoScreen : MonoBehaviour
{
    public LoadButton Slot;
    // Start is called before the first frame update
    public void CaptureSave(LoadButton saveSlot)
    {
        Slot = saveSlot;
    }

    public void OverrideSave()
    {
        Slot.SaveOverride();
    }
}
