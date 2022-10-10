using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Destructor()
    {
      
        Destroy(transform.gameObject);
    }
}
