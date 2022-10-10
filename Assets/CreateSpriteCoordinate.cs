using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CreateSpriteCoordinate : MonoBehaviour
{
    [SerializeField] private GameObject Sphere;
    [SerializeField] private float boxSize;
    [SerializeField] private float sphereSize;
    private bool outOfBounds;
    private Vector3 lastPos;
    public string Str;
    void OnDrawGizmos()
    {
        Str = "at custom (" + Sphere.transform.position.x + ", " + Sphere.transform.position.z + ")";
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(transform.position, new Vector3(boxSize, 1, boxSize));
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(Sphere.transform.position, sphereSize);
        if (Mathf.Abs(Sphere.transform.localPosition.x) > (boxSize / 2) ||
            Mathf.Abs(Sphere.transform.localPosition.z) > (boxSize / 2))
        {
            Sphere.transform.localPosition = lastPos;
        }
        else
        {
            lastPos = Sphere.transform.localPosition;
        }
    }


  

    private void Update()
    {
       
    }
}
