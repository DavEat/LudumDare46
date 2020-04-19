using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : GridEntity
{
    bool m_full = false;
    public bool full { get { return m_full; } }

    public override void RevertTurn(ITurnAction action)
    {
        m_full = false;
    }

    public void FullIt()
    {
        if (m_full)
            Debug.LogError("Pit Already full: " + m_transform.position);

        m_full = true;
        BackInTimeManager.inst.AddAction(new TurnActionFull(this));
    }
}