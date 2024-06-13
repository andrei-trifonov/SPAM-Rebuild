using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;


[System.Serializable]
public enum ThoughtType
{
    Обычная,
    Безумная,
    Пугающая,
    Мощная

}


public class Thought : MonoBehaviour
{
    public int ID;
    public string Content;
    public int Level;
    public ThoughtType Type;
    public List<Thought> toMerge = new List<Thought>();
    [SerializeField] private TextMeshPro ContentField;
    [SerializeField] private List<GameObject> Decos  = new List<GameObject>();
    [SerializeField] private List<GameObject> Levels = new List<GameObject>();
    [SerializeField] private GameObject glowEffect;
    private GameObject tmpCloseEffect;
    private GameObject MergeEffect;
    private Collider2D col;
    private bool replaceEffect;
    

    public void Initiate(string Content, int Level, ThoughtType Type, int ID, GameObject MergeEffect, bool glow)
    {
       this.ContentField.sortingOrder =  ID;
       this.GetComponent<SpriteRenderer>().sortingOrder = ID;
       this.Content = Content;
         this.Level = Level;
         this.Content = Content;
         this.Type = Type;
         this.ID = ID;
         this.MergeEffect = MergeEffect;
        ContentField.text = Content;
        Decos[(int)Type].SetActive(true);
        Levels[Level].SetActive(true);
        if (glow)
        {
            glowEffect.SetActive(true);
        }

    }

    private void Update()
    {
        if (replaceEffect)  
            MergeEffect.transform.position = transform.position +  new Vector3( col.transform.position.x - gameObject.transform.position.x, col.transform.position.y - gameObject.transform.position.y) / 2;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Brain")
        {
            if (GetComponent<DragNDropObject>().isClicked())
            {
                MergeEffect.SetActive(true);
            }

                 col = collision;
                replaceEffect = true;
                try
                {
                    Thought colThought = collision.GetComponent<Thought>();
                    if (colThought && !toMerge.Contains(colThought))
                    {
                        toMerge.Add(colThought);

                    }
                }
                catch
                {
                }
            
            
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Brain")
        {
            replaceEffect = false;
            MergeEffect.SetActive(false);
            try
            {
                if (collision.GetComponent<Thought>() && toMerge.Contains(collision.GetComponent<Thought>()))
                {
                    toMerge.Remove(collision.GetComponent<Thought>());

                }
            }
            catch
            {
            }
        }
    }

}


