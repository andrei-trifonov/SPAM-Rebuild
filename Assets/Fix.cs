using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Fix : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private bool Block;
    private float videoDuration;

    void Start()
    {
        // Получаем длительность видео
        


    }

    void Update()
    {
    if (videoPlayer.clip!= null){ 
        videoDuration = (float)videoPlayer.clip.length;//Убрать из upd
        
        float currentTime = (float)videoPlayer.time;
       

       
        if (videoPlayer.time >= videoDuration-0.2f && Block == false)
        {
            Block = true;
            videoPlayer.enabled = false;
           
            videoPlayer.enabled = true;
            
            Block = false;

        }
        }
    }

    IEnumerator BomzhCoroutine()
    {
        videoPlayer.enabled = false;
        yield return new WaitForSeconds(0.01f);
        videoPlayer.enabled = true;
        videoDuration = (float)videoPlayer.clip.length;
        Block = false;
    }

    
}