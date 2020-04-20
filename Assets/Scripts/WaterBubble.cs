using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubble : GridEntity
{
    protected override void Start()
    {
        base.Start();

        //GameManager.inst.endTurn += CheckViability;
    }
    public override void RevertTurn(ITurnAction action)
    {
        gameObject.SetActive(true);
        if (action is TurnActionPickedUp)
        {
            Debug.Log("Revert Water Picked Up");
            Score.inst.RevertScore();
        }
        else if (action is TurnActionSpread)
        {
            Debug.Log("Revert Water Spread");
            m_transform.parent.gameObject.SetActive(true);
        }
    }

    /// <summary>Check if the water bubble can still live</summary>
    void CheckViability()
    {
        /*if (crtNode.rock != null)
        {
            Spread();
        }*/
        /*else if (m_crtNode == AntMove.node)
        {
            PickUp();
        }*/
    }

    /// <summary>Player picked up the water</summary>
    public void PickUp()
    {
        GameManager.inst.endTurn -= CheckViability;
        Debug.Log("Water Picked Up");
        BackInTimeManager.inst.AddAction(new TurnActionPickedUp(this));
        Score.inst.AddScore();
        gameObject.SetActive(false);
        //Destroy(gameObject, .2f);

        SoundManager.inst.PlayWater();
    }
    /// <summary>Lose the water in the ground</summary>
    public void Spread()
    {
        GameManager.inst.endTurn -= CheckViability;
        Debug.Log("Water Spread");
        BackInTimeManager.inst.AddAction(new TurnActionSpread(this));
        gameObject.SetActive(false);
        m_transform.parent.gameObject.SetActive(false);
        //Destroy(gameObject, .2f);
    }
}
