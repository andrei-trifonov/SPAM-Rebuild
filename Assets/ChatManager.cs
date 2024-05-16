using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private GameObject Chat;
    [SerializeField] private GameObject Button;
    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject messagePrefab;
    [SerializeField] private GameObject replyPrefab;

    public void Enable()
    {
        Chat.SetActive(true);
        Button.SetActive(true);
        Panel.SetActive(true);
    }

    public void SendMessage( string message, bool answer){
        if (!Chat.activeSelf)
        {
            Enable();
        }

        if (Chat.transform.childCount>1)
            Destroy(Chat.transform.GetChild(1).gameObject);
        GameObject chatObj = Instantiate(!answer ? messagePrefab : replyPrefab, Chat.transform);
        chatObj.GetComponentInChildren<TextMeshProUGUI>().text = message;
    
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
