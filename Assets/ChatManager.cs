using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameObject Chat;
    [SerializeField] private GameObject Button;
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject replyPrefab;
    [SerializeField] private List<Sprite> Icons;
    [SerializeField] private List<string> Usernames;
    private List<GameObject> Log = new List<GameObject>();
    public void Enable()
    {
        Chat.SetActive(true);
        Button.SetActive(true);
        Panel.SetActive(true);
    }

    public void SendMessage( string message, GDB.Name author, bool answer){
        if (!Chat.activeSelf)
        {
            Enable();
        }

        if (Log.Count == 2)
        {
            Destroy(Log[0]);
            Log.RemoveAt(0);
        }

        GameObject chatObj = Instantiate(!answer ? messagePrefab : replyPrefab, Chat.transform);
        Log.Add(chatObj);
        ChatMessage CM = chatObj.GetComponent<ChatMessage>();
        CM.Avatar.sprite = Icons[(int)author];
        CM.Username.text = Usernames[(int) author];
        CM.MessageText.text = message;
    }

    public void Disable()
    {
        
        Chat.SetActive(false);
        Button.SetActive(false);
        Panel.SetActive(false);
        foreach (Transform transform in Chat.transform.GetComponentInChildren<Transform>())
        {
            Destroy(transform.gameObject);
        }
    }

}
