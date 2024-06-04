using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DestroyComponentAfterTime : MonoBehaviour
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
        toDelete.GetComponent<Animator>().enabled = false;
    }
}
