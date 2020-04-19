using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : GridEntity
{
    [SerializeField] RockData m_data = null;
    [SerializeField] RockMesh m_meshs = null;

    int m_durability = 1;
    bool m_onAPit = false;
    public bool onAPit { get { return m_onAPit; } }

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

        //crtNode.rock = this;
    }
    public override void RevertTurn(ITurnAction action)
    {
        if (action is TurnActionHit)
        {
            m_durability = ((TurnActionHit)action).durability;
        }
        else if (action is TurnActionMoveHit)
        {
            TurnActionMoveHit a = (TurnActionMoveHit)action;
            m_durability = a.durability;
            //crtNode.rock = null;

            LevelManager.inst.UpdateRockNode(crtNode, this, a.node);

            crtNode = a.node;
            m_transform.position = crtNode.worldPosition;
            //crtNode.rock = this;
        }

        if (m_onAPit)
            m_onAPit = false;

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
        //crtNode.rock = null;
        LevelManager.inst.UpdateRockNode(crtNode, this, null);
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

        //crtNode.rock = null;
        LevelManager.inst.UpdateRockNode(crtNode, this, newNode);
        crtNode = newNode;
        //crtNode.rock = this;

        Pit pit = LevelManager.inst.IsAPit(newNode);
        if (pit != null)
        {
            pit.FullIt();
            m_onAPit = true;
            m_transform.position = crtNode.worldPosition + Vector3.down;
            Debug.Log("Rock is on a pit", gameObject);
        }
        else StartCoroutine(HitAnim(crtNode, speed));

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

    IEnumerator HitAnim(Node node, float speed)
    {
        float dst;
        bool checkWaterBubble = false;

        while ((dst = (node.worldPosition - m_transform.position).sqrMagnitude) > .001f)
        {
            if (!checkWaterBubble && dst < .5f)
            {
                WaterBubble w = LevelManager.inst.IsAWaterBubble(node);
                if (w) w.Spread();
            }

            m_transform.position += ((node.worldPosition - m_transform.position).normalized * Time.deltaTime * speed);
            yield return null;
        }
        m_transform.position = node.worldPosition;
    }
}