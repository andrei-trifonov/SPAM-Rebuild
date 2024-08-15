using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class EventQTE : MonoBehaviour
{
    [SerializeField] int goalScore;
    private float curValue;
    private bool busy;
    private float clickWait;
    private NewGameCore Core;
    [SerializeField] private GameObject Effect;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject Reward;
     [SerializeField] private GameObject Clicker;
    private void OnEnable()
    {
        clickWait = 100;
        Core = GameObject.FindObjectOfType<NewGameCore>();
        Core.SetQTE(true);
    }

    public void OnClick()
    {
        if (!busy)
            StartCoroutine(ClickCoroutine());

        if (curValue >= goalScore)
        {
            GetReward();
        }
    }

    IEnumerator ClickCoroutine()
    {
        clickWait = 0;
        busy = true;    
        curValue++;
        Clicker.GetComponent<Animator>().SetBool("Play", true);
        yield return new WaitForSeconds(0.2f);
        Clicker.GetComponent<Animator>().SetBool("Play", false);
        busy = false;
    }

    private void GetReward()
    {
        
        Reward.SetActive(true);
        Clicker.SetActive(false);  
    }
    public void FinishQTE()
    {
	Reward.SetActive(false);
        Core.SetQTE(false);
        Core.EnableText(true);
	Core.Step();
    }
    private void FixedUpdate()
    {
        clickWait += 0.05f;
        if (clickWait < 1)
        {
            Effect.SetActive(true);
        }
        else
        {
            Effect.SetActive(false);
        }
        if (curValue > 0)
        {
            curValue -= 0.05f;
            progressSlider.value = curValue / goalScore;
        }
    }
}
