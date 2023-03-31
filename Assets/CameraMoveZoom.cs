using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveZoom : MonoBehaviour
{

    public Vector3 destPos;
    public float destZoom;
    [SerializeField] GameObject camPoint;
    private Camera mainCam;
    // Start is called before the first frame update
    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        destZoom = mainCam.fieldOfView;
        destPos = camPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(mainCam.fieldOfView - destZoom) > 0.5f)
        {
            if (mainCam.fieldOfView > destZoom)
            {
                mainCam.fieldOfView -= 20 * Time.deltaTime;
            }
            if (mainCam.fieldOfView < destZoom)
            {
                mainCam.fieldOfView += 20 * Time.deltaTime;
            }
        }
        if (Vector3.Distance(camPoint.transform.position, destPos)>0.5f)
        {
            camPoint.transform.position += 20 * (destPos - camPoint.transform.position).normalized * Time.deltaTime;
        }
    }
}
