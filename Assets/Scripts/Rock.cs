using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : GridEntity
{
    [SerializeField] RockData m_data = null;

    public Vector2 gridXY;

    int m_durability = 1;

    public bool breakable
    {
        get
        {
            return m_durability > 0;
        }
    }
    public int durability
    {
        get
        {
            return m_durability;
        }
    }

    protected override void Start()
    {
        base.Start();

        m_durability = m_data.durability;

        m_crtNode.rock = this;
    }
    public override void RevertTurn(ITurnAction action)
    {
        if (durability <= 0)
            gameObject.SetActive(true);

        if (action is TurnActionHit)
        {
            m_durability = ((TurnActionHit)action).durability;
        }
        else if (action is TurnActionMoveHit)
        {
            m_durability = ((TurnActionMoveHit)action).durability;
            m_crtNode = ((TurnActionMoveHit)action).node;
            m_transform.position = m_crtNode.worldPosition;
            m_crtNode.rock = this;
        }
    }

    public void Broke()
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveHit(m_crtNode, durability, this));
        m_crtNode.rock = null;
        //Destroy(gameObject, .2f);
        gameObject.SetActive(false);
    }
    public void Hit()
    {
        BackInTimeManager.inst.AddAction(new TurnActionHit(durability, this));
        HitAction();
    }
    public void Hit(Node newNode)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveHit(m_crtNode, durability, this));

        HitAction();

        m_crtNode.rock = null;
        m_crtNode = newNode;
        m_crtNode.rock = this;
        m_transform.position = m_crtNode.worldPosition;
    }

    void HitAction()
    {
        m_durability--;
    }
}