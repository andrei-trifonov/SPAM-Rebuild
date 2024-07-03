using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LockAR : MonoBehaviour
{
    // Use this for initialization
    void Start () 
    {
        Camera.main.aspect = 16f / 9f;
    }
}
