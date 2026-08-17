using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ExitEvent", menuName = "Events/ExitEvent")]
public class ExitEvent : EventData
{

    public override IEnumerator PlayEvent()
    {

        yield return base.PlayEvent();
    }
}
