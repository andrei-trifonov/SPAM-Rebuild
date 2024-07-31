using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkipButton : MonoBehaviour
{
    public NewGameCore Core;
    private Button m_button;

    public float timeInterval = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        m_button = GetComponent<Button>();
    }

    public void PointerState(bool state){
        if (state)
        {
            InvokeRepeating("CallSkip", 0f, timeInterval);
            
        }
        else
        {
            CancelInvoke("CallSkip");
            CallStop();
        }
    }

    public void CallStop()
    {
        Core.SkipUp();
    }


    public void CallSkip()
    {
        Core.SkipDown();
    }

    // Update is called once per frame

}
