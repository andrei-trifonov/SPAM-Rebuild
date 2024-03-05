using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropObject : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging;
    private InvestigationController IC;
    bool prepped_to_drop;
    private Camera ThoCam;
    [SerializeField] private GameObject TouchEffect;
    
    private void Start()
    {
        IC = GameObject.FindObjectOfType<InvestigationController>();
        ThoCam = GameObject.FindGameObjectWithTag("ThoCam").GetComponent<Camera>();
    }
    private void OnMouseDown()
    {
        transform.localScale *= 0.9f;
        TouchEffect.SetActive(true);
        _isDragging = true;
        _offset = transform.position - GetWorldPosition();
    }

    private void OnMouseUp()
    {
        transform.localScale /= 0.9f;
        TouchEffect.SetActive(false);
        _isDragging = false;
        if (prepped_to_drop) {
            IC.FinishInvestigation(GetComponent<Thought>().ID);
        }    
        else
            IC.UpdateCollisions(GetComponent<Thought>().ID);
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = GetWorldPosition() + _offset;
        }
    }

    private Vector3 GetWorldPosition()
    {
        Vector3 worldPosition = Vector3.zero;

        if (Input.touchCount > 0)
        {
            worldPosition = ThoCam.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            worldPosition = ThoCam.ScreenToWorldPoint(Input.mousePosition);
        }

        worldPosition.z = 0f; // Set the Z position to be 0 so that it matches the object's Z position
        return worldPosition;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Brain"))
        {
            prepped_to_drop = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Brain"))
        {
            prepped_to_drop = false;
        }
    }


}