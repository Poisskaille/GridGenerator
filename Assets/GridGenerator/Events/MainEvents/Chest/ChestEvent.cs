using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Chest", menuName = "Events/Chest")]
public class ChestEvent : EventData
{

    public override IEnumerator PlayEvent()
    {
        Debug.Log("Coffre");

        yield return base.PlayEvent();
    }
}
