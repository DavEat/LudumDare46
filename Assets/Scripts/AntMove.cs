using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMove : GridEntity
{
    protected override void Start()
    {
        base.Start();
    }
    public override void RevertTurn(ITurnAction action)
    {
        GoTo(((TurnActionMove)action).node);
    }

    void Update()
    {
        Inputs();
    }

    void Inputs()
    {
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
                            nextNode.rock.Broke();
                            GoTo(nextNode);
                            break;
                        }
                        else
                        {
                            Node moveBlockTo = Grid.inst.NodeFromWorldPoint(node.worldPosition + v3Dir * (distance + 1));
                            if (moveBlockTo != null && moveBlockTo.rock == null)
                            {
                                nextNode.rock.Hit(moveBlockTo);
                                GoTo(nextNode);
                            }
                            else
                            {
                                nextNode.rock.Hit();
                                GoTo(previousNode);
                            }
                        }
                        break;
                    }
                    else
                    {
                        GoTo(previousNode);
                        break;
                    }
                }
            } while (nextNode != null);
        }
    }
    void GoTo(Node node)
    {
        GameManager.inst.AddAction(new TurnActionMove(m_crtNode, this));

        m_transform.position = node.worldPosition;
        m_crtNode = node;

        if (GameManager.inst.endTurn != null)
            GameManager.inst.CallEndTurn();
    }
}
