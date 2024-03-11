using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class InvestigationBrain : MonoBehaviour
{
    private bool readyToSend;
    private GameObject obj;
    private bool readyToDrop;
    [SerializeField] private Slider Progress;
    [SerializeField] private InvestigationController IC;
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Thought"))
        {
       
            readyToSend = false;
            readyToDrop = false;
            Progress.value = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Thought"))
        {
            obj = col.gameObject;
            readyToSend = true;
        }
    }
    // Start is called before the first frame update


    // Update is called once per frame
    void FixedUpdate()
    {
        if (readyToSend && obj.GetComponent<DragNDropObject>().isDragging())
        {
            if (Progress.value < 1)
            {
                Progress.value += 0.01f;
            }
            else
            {
                readyToDrop = true;
                readyToSend = false;
            }
           
        }

        if (readyToDrop && !readyToSend && !obj.GetComponent<DragNDropObject>().isDragging())
        {
            if (IC.FinishInvestigation(obj.GetComponent<Thought>().ID)) ;
            Progress.value = 0;
            readyToDrop = false;
        
            Debug.Log("Пенис");
           // readyToSend = false;

        }
    }
}
