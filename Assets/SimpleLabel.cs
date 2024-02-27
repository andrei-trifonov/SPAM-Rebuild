using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleLabel : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text ifname;
    public TMP_Text choosename;
    public GameObject ifobj;
    public GameObject chooseobj;


    public void SetName(string text)
    {
        name.text = text;
    }

    public void SetMarker(GDB.LineType lt, string text)
    {
        Debug.Log(text);
        if (lt == GDB.LineType.If)
        {
      //     Debug.Log("Прикол");
            ifobj.SetActive(true);
            ifname.text += text;
        }
        
        if (lt == GDB.LineType.Menu)
        {
            chooseobj.SetActive(true);
            if (choosename.text!=text)
                choosename.text += text;
        }
    }
}
