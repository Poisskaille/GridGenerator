using System.Collections.Generic;
using UnityEngine;

public class EventRegistery : MonoBehaviour
{
    public static EventRegistery instance;

    [SerializeField] private List<EventData> mainEventList;

    [SerializeField] private List<EventData> subEventList;

    [SerializeField] private EventData start;
    [SerializeField] private EventData end;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    void Start()
    {
        mainEventList.Insert(0, start);
        mainEventList.Insert(1,end);

        for (int i = 0; i < mainEventList.Count; i++)
            mainEventList[i].SetID(i);

        for (int i = 0; i < subEventList.Count; i++)
            subEventList[i].SetID((i * -1) - 1);
    }

    public EventData GetRandomMainEvent() { return mainEventList[Random.Range(2, mainEventList.Count)]; }
    public EventData GetRandomSubEvent() { return subEventList[Random.Range(0, subEventList.Count)]; }
    public EventData GetMainEvent(int id) { return mainEventList[id]; }
    //public EventData GetSubEvent(int id) { return (subEventList[(id * -1) - 1]) ; }
}
