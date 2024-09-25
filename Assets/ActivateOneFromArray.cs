using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOneFromArray : MonoBehaviour
{
    [SerializeField] List<GameObject> Objects;
    // Start is called before the first frame update
    void Start()
    {


        Objects[Random.Range(0, Objects.Count)].SetActive(true);
    }

   
}
