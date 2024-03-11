using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWindows : MonoBehaviour
{
    public Material mat_on;
    public Material mat_off;
    public float minInterval = 1f; // Минимальный интервал включения/выключения света
    public float maxInterval = 5f; // Максимальный интервал включения/выключения света
    public GameObject[] lights; // Массив светов
    private int rand;
    void Start()
    {
       
        StartCoroutine(AnimateLights());
    }

    IEnumerator AnimateLights()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            rand = Random.Range(0, lights.Length);
            // Переключаем состояние каждого света в массиве
         
            if (lights[rand].GetComponent<MeshRenderer>().material.GetColor("_ColorGradient") == mat_on.GetColor("_ColorGradient"))
                lights[rand].GetComponent<MeshRenderer>().material = mat_off;
            else
            {
                lights[rand].GetComponent<MeshRenderer>().material = mat_on;
            }
        }
    }
}