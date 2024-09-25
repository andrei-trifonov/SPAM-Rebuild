using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SingletoneForIntro : MonoBehaviour
{
    // Start is called before the first frame update
    public bool intro;

    void Start()
    {
        if (intro == false)
        {
            intro = true;
        }


        DontDestroyOnLoad(gameObject);
    }

    
}
