using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : GridEntity
{
    bool m_full = false;
    public bool full { get { return m_full; } }

    bool m_cracked = false;
    public bool roofed { get { return m_roof != null && !m_cracked; } }
    [SerializeField] Animator m_roof = null;

    public override void RevertTurn(ITurnAction action)
    {
        if (action is TurnActionCrack)
        {
            m_cracked = false;
            m_roof.SetTrigger("Restor");
        }
        else m_full = false;
    }

    public void FullIt()
    {
        if (m_full)
            Debug.LogError("Pit Already full: " + m_transform.position);

        m_full = true;
        BackInTimeManager.inst.AddAction(new TurnActionFull(this));
    }

    public void CrackRoof(bool anim = true, bool revert = true)
    {
        CrackTheRoof(anim, revert, false);
    }
    public void CrackRoofFromJump(bool anim = true, bool revert = true)
    {
        CrackTheRoof(anim, revert, true);
    }
    void CrackTheRoof(bool anim = true, bool revert = true, bool jump = false)
    {
        if (!roofed) return;

        SoundManager.inst.PlayRocks(.5f);
        if (anim)
            m_roof.SetTrigger(jump ? "BreakJump" : "Break");
        else m_roof.Play("RoofDestruction", 0, 1);

        m_cracked = true;

        if (revert)
            BackInTimeManager.inst.AddAction(new TurnActionCrack(this));
    }
}