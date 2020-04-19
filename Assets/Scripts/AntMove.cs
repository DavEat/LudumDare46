using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMove : GridEntity
{
    [SerializeField] float m_moveSpeed = 1;
    float speedMul = 1f;
    bool m_animated = false;
    bool m_isInAPitOrACactus = false;

    [SerializeField] Animator m_anim = null;

    protected override void Start()
    {
        base.Start();

        if (m_anim == null)
        {
            m_anim = GetComponentInChildren<Animator>();
        }
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

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BackInTimeManager.inst.GoBackInTime();
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
    List<float> jumpdst = new List<float>();
    int totalDst = 0;
    void Move(Vector2 direction)
    {
        jumpdst.Clear();

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
                totalDst = distance;
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
                            speedMul = .3f;
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
                                    jumpdst.Add(distance);
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
        StartCoroutine(GoToActionAnim(node, rock, hit, rockMoveTo));

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
        m_transform.position = node.worldPosition + Vector3.down * .6f;
        //crtNode = node;
        Debug.Log("Fall into pit: " + node.worldPosition);
        GameManager.inst.CallEndTurn();
    }
    void FallIntoCactus(Node node)
    {
        BackInTimeManager.inst.AddAction(new TurnActionMoveRotate(crtNode, m_transform.eulerAngles.y, this));

        m_isInAPitOrACactus = true;
        m_transform.position = node.worldPosition;
        //crtNode = node;
        Debug.Log("Fall into cactus: " + node.worldPosition);
        GameManager.inst.CallEndTurn();
    }
    IEnumerator GoToActionAnim(Node node, Rock rock = null, bool hit = false, Node rockMoveTo = null)
    {
        m_animated = true;

        float dst;
        bool jumping = false;
        float jumpAt = 0;
        totalDst++;

        m_transform.rotation = Quaternion.LookRotation((node.worldPosition - m_transform.position), Vector3.up);

        m_anim.SetFloat("Speed", m_moveSpeed * speedMul);

        while ((dst = (m_transform.position - node.worldPosition).sqrMagnitude) > .001f)
        {
            if (rock != null) //rock anim
            {
                if (hit)
                {
                    if (dst < Grid.inst.nodeRadius * 1.8f)
                    {
                        rock.Hit(rockMoveTo, m_moveSpeed);
                        rock = null;
                    }
                }
                else if (dst < Grid.inst.nodeRadius * 1f)
                {
                    rock.Broke();
                    rock = null;
                }
            }

            float dstNsqr = (m_transform.position - node.worldPosition).magnitude;
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
                foreach (float j in jumpdst)
                {
                    if (dstNsqr < (totalDst - (j + .55f)) * Grid.inst.nodeRadius * 2)
                    {
                        m_anim.SetBool("Jump", true);
                        m_anim.SetTrigger("JumpTrigger");
                        jumping = true;
                        jump = j;
                        jumpAt = ((totalDst - (j + 1.1f))) * Grid.inst.nodeRadius * 2;
                    }
                }
                if (jump != -1) jumpdst.Remove(jump);
            }

            m_transform.position += ((node.worldPosition - m_transform.position).normalized * Time.deltaTime * m_moveSpeed * speedMul);
            yield return null;
        }

        m_transform.position = node.worldPosition;
        crtNode = node;

        speedMul = 1f;
        m_anim.SetFloat("Speed", 0);
        m_anim.SetBool("Jump", false);
        m_animated = false;

        CheckVictoryCollectibles(node);
        GameManager.inst.CallEndTurn();
    }
}
