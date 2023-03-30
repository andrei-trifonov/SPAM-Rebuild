using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[System.Serializable]
public enum ThoughtType
{
    Обычная,
    Мощная,
    Безумная,
    Пугающая

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
    public void Initiate(string Content, int Level, ThoughtType Type, int ID)
    {
         this.Content = Content;
         this.Level = Level;
         this.Content = Content;
         this.Type = Type;
         this.ID = ID;
        ContentField.text = Content;
        Decos[(int)Type].SetActive(true);
        Levels[Level].SetActive(true);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        try
        {
            Thought colThought = collision.GetComponent<Thought>();
            if (colThought && !toMerge.Contains(colThought))
            {
                toMerge.Add(colThought);

            }
        }
        catch { }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        try
        {
            if (collision.GetComponent<Thought>() && toMerge.Contains(collision.GetComponent<Thought>()))
            {
                toMerge.Remove(collision.GetComponent<Thought>());

            }
        }
        catch { }
    }

}


