using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMove : GridEntity
{
    [SerializeField] float m_moveSpeed = 1;
    float speedMul = 1f;
    bool m_animated = false;
    bool m_isInAPit = false;

    protected override void Start()
    {
        base.Start();
    }
    public override void RevertTurn(ITurnAction action)
    {
        if (m_isInAPit)
            m_isInAPit = false;

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

        if (m_isInAPit) return;

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

    void Move(Vector2 direction)
    {
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

            int distance = 0;
            Node nextNode = null, previousNode = node;
            do
            {
                distance++;
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
                                GoTo(nextNode, nextRock, true, moveBlockTo);
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
                                Debug.Log("Jump Above Pit: " + nextNode.worldPosition, nextPit.gameObject);
                            }
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

        m_isInAPit = true;
        m_transform.position = node.worldPosition + Vector3.down * .6f;
        //crtNode = node;
        Debug.Log("Fall into pit: " + node.worldPosition);
        GameManager.inst.CallEndTurn();
    }
    IEnumerator GoToActionAnim(Node node, Rock rock = null, bool hit = false, Node rockMoveTo = null)
    {
        m_animated = true;

        float dst;

        m_transform.rotation = Quaternion.LookRotation((node.worldPosition - m_transform.position), Vector3.up);

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

            m_transform.position += ((node.worldPosition - m_transform.position).normalized * Time.deltaTime * m_moveSpeed * speedMul);
            yield return null;
        }

        m_transform.position = node.worldPosition;
        crtNode = node;

        speedMul = 1f;
        m_animated = false;

        CheckVictoryCollectibles(node);
        GameManager.inst.CallEndTurn();
    }
}
