using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveZoom : MonoBehaviour
{

   private Vector3 destPos;
    public int Speed;
    private  float destZoom;
    [SerializeField] GameObject camPoint;
    private Camera mainCam;
    private Vector3 startPos;
  
    // Start is called before the first frame update
    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        destZoom = mainCam.fieldOfView;
        destPos = camPoint.transform.position;
        startPos = camPoint.transform.position;
    }

    public void NewMove(Vector3 pos, float zoom){
        startPos = camPoint.transform.position;
        destPos  = pos;
        destZoom = zoom;
    }
    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(mainCam.fieldOfView - destZoom) > 0.5f)
        {
            if (mainCam.fieldOfView > destZoom)
            {
                mainCam.fieldOfView -=Speed * Time.deltaTime;
            }
            if (mainCam.fieldOfView < destZoom)
            {
                mainCam.fieldOfView += Speed * Time.deltaTime;
            }
        }
        if (Vector3.Distance(camPoint.transform.position, destPos)>0.5)
        {
                if ((destPos - camPoint.transform.position).magnitude > 0)
            camPoint.transform.position+= Speed * (destPos - startPos).normalized ;
            else
             camPoint.transform.position-= Speed * (destPos - startPos).normalized ;
        }
    }
}
