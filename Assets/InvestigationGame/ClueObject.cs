using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueObject : MonoBehaviour
{
    public ThoughtSt Thought;
    private InvestigationController IC;
    

    private void Start()
    {
        IC = GameObject.FindObjectOfType<InvestigationController>();
    }
    private void OnMouseDown()
    {

        IC.AddThought(Thought);
        Destroy(this);
    }


    }