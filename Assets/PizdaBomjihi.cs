using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class PizdaBomjihi : MonoBehaviour
{
    private VideoPlayer VP;
    // Start is called before the first frame update
    void Start()
    {
        VP = GetComponent<VideoPlayer>();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
         if (!VP.isPlaying)
            VP.Play();        
    }
}
