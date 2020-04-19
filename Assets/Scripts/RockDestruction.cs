using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDestruction : GridEntity
{
    protected override void Start()
    {
        BackInTimeManager.inst.AddAction(new TurnActionDestroy(this));
    }

    public override void RevertTurn(ITurnAction action)
    {
        Destroy(gameObject);
    }
}
