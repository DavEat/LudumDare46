using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMove : GridEntity
{
    [SerializeField] float m_moveSpeed = 1;
    float speedMul = 1f;
    bool m_animated = false;

    protected override void Start()
    {
        base.Start();
    }
    public override void RevertTurn(ITurnAction action)
    {
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
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BackInTimeManager.inst.GoBackInTime();
        }
    }

    void Move(Vector2 direction)
    {
        Vector3 v3Dir = new Vector3(direction.x, 0, direction.y);
        Node node = Grid.inst.NodeFromWorldPoint(m_transform.position + v3Dir);

        if (node != null)
        {
            if (!node.walkable || node.rock != null) return; //can't push block

            int distance = 0;
            Node nextNode = null, previousNode = node;
            do
            {
                distance++;
                if (nextNode != null) previousNode = nextNode;

                nextNode = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance));

                if (nextNode == null)
                    break;

                if (nextNode.rock != null)
                {
                    if (nextNode.rock.breakable && distance > 1)
                    {
                        if (nextNode.rock.durability == 1)
                        {
                            //nextNode.rock.Broke();
                            GoTo(nextNode, nextNode.rock, false);
                            break;
                        }
                        else
                        {
                            Node moveBlockTo = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance + 1));
                            if (moveBlockTo != null && moveBlockTo.rock == null)
                            {
                                //nextNode.rock.Hit(moveBlockTo);
                                GoTo(nextNode, nextNode.rock, true, moveBlockTo);
                            }
                            else
                            {
                                //nextNode.rock.Hit();
                                GoTo(previousNode, nextNode.rock, true);
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
