using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubble : GridEntity
{
    protected override void Start()
    {
        base.Start();

        GameManager.inst.endTurn += CheckViability;
    }
    public override void RevertTurn(ITurnAction action)
    {
        gameObject.SetActive(true);
        if (action is TurnActionPickedUp)
        {
            Debug.Log("Revert Water Picked Up");
        }
        else if (action is TurnActionSpread)
        {
            Debug.Log("Revert Water Spread");
        }
    }

    /// <summary>Check if the water bubble can still live</summary>
    void CheckViability()
    {
        if (m_crtNode.rock != null)
        {
            Spread();
        }
        /*else if (m_crtNode == AntMove.node)
        {
            PickUp();
        }*/
    }

    /// <summary>Player picked up the water</summary>
    void PickUp()
    {
        GameManager.inst.endTurn -= CheckViability;
        Debug.Log("Water Picked Up");
        BackInTimeManager.inst.AddAction(new TurnActionPickedUp(this));
        //Destroy(gameObject, .2f);
        gameObject.SetActive(false);
    }
    /// <summary>Lose the water in the ground</summary>
    void Spread()
    {
        GameManager.inst.endTurn -= CheckViability;
        Debug.Log("Water Spread");
        BackInTimeManager.inst.AddAction(new TurnActionSpread(this));
        //Destroy(gameObject, .2f);
        gameObject.SetActive(false);
    }
}
