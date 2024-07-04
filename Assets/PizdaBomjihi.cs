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
        // Обновляем таймер
        float currentTime = (float)videoPlayer.time;
       
        Debug.Log("Time " +videoPlayer.time );
        Debug.Log("Duration " + videoDuration );
        // Проверяем, закончилось ли видео
        videoDuration = (float)videoPlayer.clip.length;
        if (videoPlayer.time >= videoDuration-0.5f && Block == false)
        {
            Block = true;
            // Отключаем компонент Video Player
            StartCoroutine(BomzhCoroutine());

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