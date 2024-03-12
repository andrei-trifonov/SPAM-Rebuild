using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Track
{
    public string Name;
    public string Author;
};
public class OSTList : MonoBehaviour
{
    [SerializeField] private List<Track> TrackList = new List<Track>();

    public List<Track> GetTrackList()
    {
        return TrackList;
    }
}
