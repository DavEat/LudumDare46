using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : GridEntity
{
    public override void RevertTurn(ITurnAction action)
    {
        Debug.LogError("Cactus should not be there");
    }
}
