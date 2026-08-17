using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Objects/EventData")]
public class EventData : ScriptableObject
{
    public virtual IEnumerator PlayEvent()
    {
        eventFinished = true;
        yield return null; 
    }

    [HideInInspector]
    public bool eventFinished = false;

    private int eventID = -1;
    public bool canPlay = true;

    public GameObject prefab;

    //[Header("Dialogue will play before the event")]
    //public DialogueData dialogue;

    public int GetID() {  return eventID; }
    public void SetID(int id) {  eventID = id; } 
}
