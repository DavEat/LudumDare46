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
            else if (m_durability == 3)
            {
                GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(false, false, true);
            }
        }
    }

    public void Broke(Vector3 antForward)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveHit(crtNode, durability, this));
        //crtNode.rock = null;
        LevelManager.inst.UpdateRockNode(crtNode, this, null);
        m_durability = 0;
        //Destroy(gameObject, .2f);
        gameObject.SetActive(false);
        if (m_meshs != null)
        {
            m_meshs.SpawnDestruction(m_transform.position, antForward);
        }

        //SoundManager.inst.PlayRocks();
        SoundManager.inst.PlayRocks(1f);
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
        //LevelManager.inst.UpdateRockNode(crtNode, this, newNode);
        //crtNode = newNode;
        //crtNode.rock = this;

        Pit pit = LevelManager.inst.IsAPit(newNode);
        if (pit != null)
        {
            pit.FullIt();
            m_onAPit = true;
            //m_transform.position = crtNode.worldPosition + Vector3.down;
            Debug.Log("Rock is on a pit", gameObject);
            StartCoroutine(HitAnim(newNode, speed, true));
        }
        else
        {
            StartCoroutine(HitAnim(newNode, speed, false));
        }

        //m_transform.position = crtNode.worldPosition;
    }

    void HitAction()
    {
        m_durability--;
        if (m_meshs != null)
        {
            if (m_durability == 1)
                GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(true, false);
            else if(m_durability == 2)
                GetComponent<MeshFilter>().sharedMesh = m_meshs.GetMesh(false, false);
        }

        SoundManager.inst.PlayRocks(.7f);
    }

    IEnumerator HitAnim(Node node, float speed, bool isApit)
    {
        //float dst;
        bool checkWaterBubble = false;

        Vector2 nodePos = new Vector2(node.worldPosition.x, node.worldPosition.z);
        Vector2 rockPos;
        Vector2 startRockPos = new Vector2(m_transform.position.x, m_transform.position.z);

        float totalDistance = (startRockPos - nodePos).sqrMagnitude - .1f;

        bool forward = true;

        while (forward)
        {
            rockPos = new Vector2(m_transform.position.x, m_transform.position.z);
            if ((startRockPos - rockPos).sqrMagnitude > totalDistance)
            {
                forward = false;
            }

            if (!checkWaterBubble && (rockPos - nodePos).sqrMagnitude < .5f)
            {
                WaterBubble w = LevelManager.inst.IsAWaterBubble(node);
                if (w) w.Spread();
            }

            if (isApit)
            {
                float d = (nodePos - rockPos).magnitude;
                if (d < .3f)
                {
                    float y = Mathf.Lerp(m_transform.position.y, -1.0f, 1 - (d / .3f));
                    m_transform.position = new Vector3(m_transform.position.x, y, m_transform.position.z);
                }
            }

            m_transform.position += ((node.worldPosition - m_transform.position).normalized * Time.deltaTime * speed);
            yield return null;
        }
        //m_transform.position = node.worldPosition;
        m_transform.position = new Vector3(node.worldPosition.x, isApit ? -1.0f : 0, node.worldPosition.z);

        LevelManager.inst.UpdateRockNode(crtNode, this, isApit ? null : node);
        crtNode = isApit ? null : node;
    }
}