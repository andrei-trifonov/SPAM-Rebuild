using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public float maxVolumeMusic;
    public bool fadingIn;
    public bool fadingOut;
    // Update is called once per frame
    void FixedUpdate()
    {
        float volume =  gameObject.GetComponent<AudioSource>().volume;
        if (fadingIn)
        {
            if (volume < maxVolumeMusic)
                gameObject.GetComponent<AudioSource>().volume += 0.005f;
            else
                fadingIn = false;
        }

        if (fadingOut)
        {
            if (fadingIn)
                Destroy(this.gameObject);
            if (volume > 0) 
                gameObject.GetComponent<AudioSource>().volume -= 0.005f;
            else
            {
                fadingOut = false;
                gameObject.GetComponent<AudioSource>().Stop();
                Destroy(this.gameObject);
            }
        }    
    }
}
