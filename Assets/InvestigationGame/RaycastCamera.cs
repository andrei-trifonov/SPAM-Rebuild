using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RaycastCamera : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
#else
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        
            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ignore Raycast"))) {
                Transform objectHit = hit.transform;
                if (objectHit.GetComponent<RaycastObject>())
                {
                    objectHit.GetComponent<RaycastObject>().Activate();
                }

                // Do something with the object that was hit by the raycast.
            }
        }
    }
}