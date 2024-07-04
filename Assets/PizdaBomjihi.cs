using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PizdaBomjihi : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Ссылка на Video Player
    private bool Block;
    private float videoDuration;

    void Start()
    {
        // Получаем длительность видео
        


    }

    void Update()
    {
    if (videoPlayer.clip!= null){ 
        videoDuration = (float)videoPlayer.clip.length;
        // Обновляем таймер
        float currentTime = (float)videoPlayer.time;
       
        //Debug.Log("Time " +videoPlayer.time );
        //Debug.Log("Duration " + videoDuration );
        // Проверяем, закончилось ли видео
       
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
    // Форматирование времени в формате "ММ:СС"
    
}