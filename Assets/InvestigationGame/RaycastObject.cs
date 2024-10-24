
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum buttons
{
 Ep1,
 Ep2,
 Ep3,
 Gallery,
 Settings

}
public class RaycastObject : MonoBehaviour
{
    public buttons b_enum;
    public Gallery Gallery;
    public List<RaycastObject> RaycastObjects;

    public GameObject Settings;
    public Canvas Preloader;
    public GameObject PreloaderObj;
    public VideoPlayer videoPlayer; // Ссылка на VideoPlayer
    public AudioSource mainMenuMusic;
    public Volume cameraEffects;


    void OnVideoEnd(VideoPlayer vp)
    {
       
        PlayerPrefs.SetInt("Opening",1);
        Debug.Log("Видео закончилось!");
        cameraEffects.enabled = true;
        videoPlayer.enabled = false;
        StartCoroutine(LoadSceneCoroutine());

    
       
    }
    IEnumerator LoadSceneCoroutine()
    {
       
        Preloader.enabled = true;
        PreloaderObj.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("FinalGameplayScene");
    }

    IEnumerator OpeningCoroutine()
    {
        cameraEffects.enabled = false;
        mainMenuMusic.Stop();
        ResourceRequest request = Resources.LoadAsync<VideoClip>("3DBG/Opening");
                                       
        while (!request.isDone)
        {
            yield return null;
        }
                                       
        if (request.asset == null)
        {
            Debug.LogError("Failed to load " );
        }
        else
        { 
            VideoClip res = request.asset as VideoClip;
            videoPlayer.clip = res;
            videoPlayer.Play();


        }
        
        

        
   
                    
                    
                    
                    
             
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    public virtual void Activate()
    {
        switch (b_enum)
        {
            case buttons.Ep1:
            {
                if (PlayerPrefs.GetInt("Opening")==0)
                {


                    StartCoroutine(OpeningCoroutine());
                  
                    
                }
                else
                {
                    StartCoroutine(LoadSceneCoroutine());
                }

            }
                break;
            case buttons.Gallery:
            {
                Gallery.SetGallery(true);
                GameObject c = GameObject.FindWithTag("MainCamera");
                c.GetComponent<RaycastCamera>().enabled = false;

            }
                break;
            case buttons.Settings:
            {
                Settings.SetActive(true);
                GameObject c = GameObject.FindWithTag("MainCamera");
                c.GetComponent<RaycastCamera>().enabled = false;

            }
                break;
        }

        
    }

    public void Return()
    {
        
        GameObject c = GameObject.FindWithTag("MainCamera");
        c.GetComponent<RaycastCamera>().enabled = true;
    }

}
