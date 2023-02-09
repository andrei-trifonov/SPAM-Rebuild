using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
 struct Quest
{
    public string questName;
    public string jumpName;
    public List<string> Resources;
    
    public Quest(string questName, string jumpName, List<string> Resources)
    {
        this.jumpName = jumpName;
        this.Resources = Resources;
        this.questName = questName;
    }
}
public class MapPoint : MonoBehaviour
{
    private bool hasMainQuest;
    [SerializeField] private Core gameCore;
    [SerializeField] private List<Quest> pointQuests;
    private Quest mainQuest;

    public void AddMainQuest( string jumpName, List<string> Resources)
    {
        hasMainQuest = true;
        mainQuest = new Quest("Main", jumpName, Resources);
    }
    // Update is called once per frame
    public void OnClick()
    {
        
        if (hasMainQuest)
        {
            hasMainQuest = false;
            gameCore.LoadMediaCluster(mainQuest.Resources);
            gameCore.outerJumpLabel(mainQuest.jumpName);
        }
        else
        {
            gameCore.LoadMediaCluster(pointQuests[0].Resources);
            gameCore.outerJumpLabel(pointQuests[0].jumpName);
            pointQuests.RemoveAt(0);
        }

        

    }
}
