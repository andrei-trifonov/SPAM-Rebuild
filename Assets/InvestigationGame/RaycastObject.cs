
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    IEnumerator LoadSceneCoroutine()
    {
        Preloader.enabled = true;
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("FinalGameplayScene");
    }

    public virtual void Activate()
    {
        switch (b_enum)
        {
            case buttons.Ep1:
            {
                
                StartCoroutine(LoadSceneCoroutine());
                
                
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
