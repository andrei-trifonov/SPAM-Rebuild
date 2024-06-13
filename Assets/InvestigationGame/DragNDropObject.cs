using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNDropObject : MonoBehaviour
{
 
    private Vector3 _offset;
    private bool _isDragging;
    private bool Clicked;
    private InvestigationController IC;

    private Camera ThoCam;
    [SerializeField] private GameObject TouchEffect;

    public bool isDragging()
    {
        return _isDragging;
    }
    public bool isClicked()
    {
        return Clicked;
    }
    private void Start()
    {
        IC = GameObject.FindObjectOfType<InvestigationController>();
        ThoCam = GameObject.FindGameObjectWithTag("ThoCam").GetComponent<Camera>();
    }
    private void OnMouseDown()
    {
        Clicked = true;
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
        if (GetComponent<Thought>().toMerge.Count > 0)
        {
            IC.UpdateCollisions(GetComponent<Thought>().ID, GetComponent<Thought>().toMerge[0].ID);
            Debug.Log(GetComponent<Thought>().toMerge.Count );
        }
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




}