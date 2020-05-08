using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMove : GridEntity
{
    [SerializeField] float m_moveSpeed = 1;
    [SerializeField] float m_fallSpeed = 5;
    float speedMul = 1f;
    bool m_animated = false;
    bool m_isInAPitOrACactus = false;

    [SerializeField] Animator m_anim = null;

    List<float> m_jumpdst = new List<float>();
    int m_totalDst = 0;

    float m_antYPos = 0;
    float m_antInAPitYPos = -.6f;


    protected override void Start()
    {
        base.Start();

        if (m_anim == null)
        {
            m_anim = GetComponentInChildren<Animator>();
        }

        StartCoroutine(DropAntAnim());
    }
    public override void RevertTurn(ITurnAction action)
    {
        if (m_isInAPitOrACactus)
            m_isInAPitOrACactus = false;

        TurnActionMoveRotate a = (TurnActionMoveRotate)action;
        m_transform.eulerAngles = Vector3.up * a.angleY;
        GoToAction(a.node);
    }

    void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (m_animated) return;

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            BackInTime();
            return;
        }

        if (m_isInAPitOrACactus) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector2.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector2.left);
        }
    }

    public void BackInTime()
    {
        BackInTimeManager.inst.GoBackInTime();
    }
    public void Move(int direction)
    {
        switch(direction)
        {
            case 0:
                Move(Vector2.up);
                break;
            case 1:
                Move(Vector2.down);
                break;
            case 2:
                Move(Vector2.right);
                break;
            case 3:
                Move(Vector2.left);
                break;
        }
    }

    public void Move(Vector2 direction)
    {
        m_jumpdst.Clear();

        Vector3 v3Dir = new Vector3(direction.x, 0, direction.y);
        Node node = Grid.inst.NodeFromWorldPoint(m_transform.position + v3Dir);

        if (node != null)
        {
            Rock nodeRock = LevelManager.inst.IsARock(node);

            if (!node.walkable || nodeRock != null) return; //can't push block

            Pit nodePit = LevelManager.inst.IsAPit(node);
            if (nodePit != null)
            {
                FallIntoPit(node);
                return;
            }
            Cactus nodeCactus = LevelManager.inst.IsACactus(node);
            if (nodeCactus != null)
            {
                FallIntoCactus(node);
                return;
            }

            int distance = 0;
            Node nextNode = null, previousNode = node;
            do
            {
                distance++;
                m_totalDst = distance;
                if (nextNode != null) previousNode = nextNode;

                nextNode = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance));

                if (nextNode == null || nextNode == previousNode)
                    break;

                Rock nextRock = LevelManager.inst.IsARock(nextNode);

                if (nextRock != null)
                {
                    if (nextRock.breakable && distance > 1)
                    {
                        if (nextRock.durability == 1)
                        {
                            //nextRock.Broke();
                            GoTo(nextNode, nextRock, false);
                            break;
                        }
                        else
                        {
                            Node moveBlockTo = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance + 1));
                            Rock moveBlockToRock = LevelManager.inst.IsARock(moveBlockTo);

                            if (moveBlockTo != null && moveBlockToRock == null)
                            {
                                //nextRock.Hit(moveBlockTo);
                                Cactus nextCactus = LevelManager.inst.IsACactus(moveBlockTo);
                                if (nextCactus != null)
                                     GoTo(previousNode, nextRock, true);
                                else GoTo(nextNode, nextRock, true, moveBlockTo);
                            }
                            else
                            {
                                //nextRock.Hit();
                                GoTo(previousNode, nextRock, true);
                            }
                        }
                        break;
                    }
                    else
                    {
                        if (distance == 1)
                        {
                            Debug.Log("Move one");
                            speedMul = .5f;
                        }

                        GoTo(previousNode);
                        break;
                    }
                }
                else
                {
                    Pit nextPit = LevelManager.inst.IsAPit(nextNode);
                    if (nextPit != null)
                    {
                        Node nextnextNode = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance + 1));
                        Pit nextnextPit = LevelManager.inst.IsAPit(nextnextNode);

                        if (nextnextPit != null)
                        {
                            FallIntoPit(nextnextNode);
                            return;
                        }
                        else
                        {
                            Rock nextnextRock = LevelManager.inst.IsARock(nextnextNode);
                            if (nextnextRock != null)
                            {
                                FallIntoPit(nextNode);
                                return;
                            }
                            else
                            {
                                Cactus nextCactus = LevelManager.inst.IsACactus(nextNode);
                                if (nextCactus != null)
                                {
                                    FallIntoCactus(nextNode);
                                    return;
                                }
                                else
                                {
                                    m_jumpdst.Add(distance);
                                    Debug.Log("Jump Above Pit: " + nextNode.worldPosition, nextPit.gameObject);
                                }
                            }
                        }
                    }
                    else
                    {
                        Cactus nextCactus = LevelManager.inst.IsACactus(nextNode);
                        if (nextCactus != null)
                        {
                            FallIntoCactus(nextNode);
                            return;
                        }
                    }
                }
            } while (nextNode != null);

            if (nextNode == null)
            {
                Debug.Log("Move null");
                GoTo(previousNode);
            }
        }
    }
    void GoTo(Node node, Rock rock = null, bool hit = false, Node rockMoveTo = null)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveRotate(crtNode, m_transform.eulerAngles.y, this));

        //GoToAction(node);
        StartCoroutine(GoToActionAnim(node, MoveStatus.rock, rock, hit, rockMoveTo));

    }
    void GoToAction(Node node)
    {
        m_transform.position = node.worldPosition;
        crtNode = node;
    }
    void CheckVictoryCollectibles(Node node)
    {
        if (ReachEndLevel(node))
        { }
        else ReachCollectible(node);
    }
    bool ReachEndLevel(Node node)
    {
        Door d = LevelManager.inst.IsADoor(node);
        if (d != null)
        {
            Debug.Log("Win go next");
            d.Active();
            return true;
        }
        return false;
    }
    bool ReachCollectible(Node node)
    {
        WaterBubble w = LevelManager.inst.IsAWaterBubble(node);
        if (w != null)
        {
            w.PickUp();
            return true;
        }
        return false;
    }
    void FallIntoPit(Node node)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveRotate(crtNode, m_transform.eulerAngles.y, this));

        m_isInAPitOrACactus = true;
        //m_transform.position = node.worldPosition + Vector3.down * .6f;
        //crtNode = node;
        Debug.Log("Fall into pit");
        StartCoroutine(GoToActionAnim(node, MoveStatus.pit));
    }
    void FallIntoCactus(Node node)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveRotate(crtNode, m_transform.eulerAngles.y, this));

        m_isInAPitOrACactus = true;
        //m_transform.position = node.worldPosition;
        //crtNode = node;
        Debug.Log("Fall into cactus");
        StartCoroutine(GoToActionAnim(node, MoveStatus.cactus));
    }
    enum MoveStatus { none, rock, cactus, pit }
    IEnumerator GoToActionAnim(Node node, MoveStatus status = MoveStatus.rock, Rock rock = null, bool hit = false, Node rockMoveTo = null)
    {
        m_animated = true;

        //float dst;
        bool jumping = false;
        float jumpAt = 0;
        m_totalDst++;
        bool inLoseAnim = false;

        m_transform.rotation = Quaternion.LookRotation((node.worldPosition - m_transform.position), Vector3.up);

        m_anim.SetFloat("Speed", m_moveSpeed * speedMul);

        Vector2 nodePos = new Vector2(node.worldPosition.x, node.worldPosition.z);
        Vector2 antPos;
        Vector2 startAntPos = new Vector2(m_transform.position.x, m_transform.position.z);

        float totalDistance = (nodePos - startAntPos).sqrMagnitude - .002f;

        bool forward = true;
        //while ((dst = ((antPos = new Vector2(m_transform.position.x, m_transform.position.z)) - nodePos).sqrMagnitude) > .001f)
        while (forward)
        {
            antPos = new Vector2(m_transform.position.x, m_transform.position.z);

            if ((startAntPos - antPos).sqrMagnitude > totalDistance)
            {
                forward = false;
            }

            //dst = (startAntPos - antPos).sqrMagnitude - (nodePos - startAntPos).sqrMagnitude;

            SoundManager.inst.PlayFoot(speedMul);

            float dstNsqr = (antPos - nodePos).magnitude;
            if (!inLoseAnim)
            {
                if (rock != null) //rock anim
                {
                    if (hit)
                    {
                        if (dstNsqr < Grid.inst.nodeRadius * 1.8f)
                        {
                            rock.Hit(rockMoveTo, m_moveSpeed);
                            rock = null;
                        }
                    }
                    else if (dstNsqr < Grid.inst.nodeRadius * 1f)
                    {
                        rock.Broke(m_transform.forward);
                        rock = null;
                    }
                }
                
                if (jumping)
                {
                    if (dstNsqr < jumpAt)
                    {
                        jumping = false;
                        m_anim.SetBool("Jump", false);
                    }
                }
                else
                {
                    float jump = -1;
                    foreach (float j in m_jumpdst)
                    {
                        if (dstNsqr < (m_totalDst - (j + .55f)) * Grid.inst.nodeRadius * 2)
                        {
                            m_anim.SetBool("Jump", true);
                            m_anim.SetTrigger("JumpTrigger");
                            jumping = true;
                            jump = j;
                            jumpAt = ((m_totalDst - (j + 1.1f))) * Grid.inst.nodeRadius * 2;

                            SoundManager.inst.PlayJump();
                        }
                    }
                    if (jump != -1) m_jumpdst.Remove(jump);
                }
                if (status == MoveStatus.cactus)
                {
                    if (dstNsqr < .6f * Grid.inst.nodeRadius * 2)
                    {
                        inLoseAnim = true;
                        totalDistance -= .1f;
                        Debug.Log("cactus anim");
                    }
                }
                else if (status == MoveStatus.pit)
                {
                    if (dstNsqr < .4f * Grid.inst.nodeRadius * 2)
                    {
                        inLoseAnim = true;
                        totalDistance -= .1f;
                        Debug.Log("pit anim");
                    }
                }
            }
            else
            {
                if (status == MoveStatus.pit)
                {
                    float y = Mathf.Lerp(m_transform.position.y, m_antInAPitYPos, 1 - (dstNsqr / .4f));
                    m_transform.position = new Vector3(m_transform.position.x, y, m_transform.position.z);
                }
            }

            m_transform.position += ((node.worldPosition - m_transform.position).normalized * Time.deltaTime * m_moveSpeed * speedMul);
            yield return null;
        }

        m_transform.position = new Vector3(node.worldPosition.x, status == MoveStatus.pit ? m_antInAPitYPos : m_antYPos, node.worldPosition.z);
        crtNode = node;

        speedMul = 1f;
        m_anim.SetFloat("Speed", 0);
        m_anim.SetBool("Jump", false);
        m_animated = false;

        CheckVictoryCollectibles(node);
        GameManager.inst.CallEndTurn();
    }
    IEnumerator DropAntAnim()
    {
        m_animated = true;
        m_transform.position = new Vector3(m_transform.position.x, 10, m_transform.position.z);

        yield return null;
        
        while (m_transform.position.y > 0)
        {
            m_transform.position -= Vector3.up * m_fallSpeed * Time.deltaTime;
            yield return null;
        }

        m_transform.position = new Vector3(m_transform.position.x, 0, m_transform.position.z);
        m_animated = false;
    }
}
