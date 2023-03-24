using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPoint : MonoBehaviour
{

    [SerializeField] Transform Point;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(Point);
    }
}
