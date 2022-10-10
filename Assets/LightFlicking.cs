using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicking : MonoBehaviour
{
    // Start is called before the first frame update
    private Light FlickLight;
    void Start()
    {
        FlickLight = GetComponent<Light>();
        StartCoroutine(FlickLightCoroutine(Random.Range(0.5f, 10f)));
        
    }


    IEnumerator FlickLightCoroutine(float randomTime)
    {
        
        yield return new WaitForSeconds(randomTime);
        FlickLight.enabled = false;
        yield return new WaitForSeconds(1);
        FlickLight.enabled = true;
        FlickLightCoroutine(Random.Range(0.5f, 10f));
    }   
    // Update is called once per frame

}
