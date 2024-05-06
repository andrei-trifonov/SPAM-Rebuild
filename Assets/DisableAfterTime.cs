using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public GameObject toDelete;
    public float Time;
    void Start()
    {
        StartCoroutine(SomeCoroutine());
    }
    public void SomeMethod()
    {
        
    }
    private IEnumerator SomeCoroutine()
    {
      
        yield return new WaitForSeconds (Time);
        Destroy(toDelete);
    }
}
