using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : GridEntity
{
    [SerializeField] RockData m_data = null;
    [SerializeField] RockMesh m_meshs = null;

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

        crtNode.rock = this;
    }
    public override void RevertTurn(ITurnAction action)
    {
        if (action is TurnActionHit)
        {
            m_durability = ((TurnActionHit)action).durability;
        }
        else if (action is TurnActionMoveHit)
        {
            m_durability = ((TurnActionMoveHit)action).durability;
            crtNode.rock = null;
            crtNode = ((TurnActionMoveHit)action).node;
            m_transform.position = crtNode.worldPosition;
            crtNode.rock = this;
        }

        if (durability > 0)
            gameObject.SetActive(true);

        if (m_meshs != null)
        {
            if (m_durability == 1)
            {
                GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(true, m_data.durability == 1);
            }
            else if (m_durability == 2)
            {
                GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(false, false);
            }
        }
    }

    public void Broke()
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveHit(crtNode, durability, this));
        crtNode.rock = null;
        m_durability = 0;
        //Destroy(gameObject, .2f);
        gameObject.SetActive(false);
    }
    public void Hit()
    {
        BackInTimeManager.inst.AddAction(new TurnActionHit(durability, this));
        HitAction();
    }
    public void Hit(Node newNode, float speed)
    {
        if (newNode == null)
        {
            Hit();
            return;
        }

        BackInTimeManager.inst.AddAction(new TurnActionMoveHit(crtNode, durability, this));

        HitAction();

        crtNode.rock = null;
        crtNode = newNode;
        crtNode.rock = this;

        StartCoroutine(HitAnim(crtNode, speed));

        //m_transform.position = crtNode.worldPosition;
    }

    void HitAction()
    {
        m_durability--;
        if (m_durability == 1 && m_meshs != null)
        {
            GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(true, false);
        }
    }

    IEnumerator HitAnim(Node position, float speed)
    {
        while ((position.worldPosition - m_transform.position).sqrMagnitude > .001f)
        {
            m_transform.position += ((position.worldPosition - m_transform.position).normalized * Time.deltaTime * speed);
            yield return null;
        }
        m_transform.position = position.worldPosition;
    }
}