using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOutline : AddOutline
{
    protected override bool ShouldShow()
    {
        return missionManager.Value.ObjectsToSteal - missionManager.Value.ObjectsStolen <= 0;
    }
}
