using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropObject : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging;
    private InvestigationController IC;
    bool prepped_to_drop;
    
    private void Start()
    {
        IC = GameObject.FindObjectOfType<InvestigationController>();
    }
    private void OnMouseDown()
    {
        _isDragging = true;
        _offset = transform.position - GetWorldPosition();
    }

    private void OnMouseUp()
    {
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
            worldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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