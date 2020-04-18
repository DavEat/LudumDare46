using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = _grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = _grid.NodeFromWorldPoint(request.pathEnd);

        if (/*startNode.walkable &&*/ targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node crtNode = openSet.RemoveFirst();
                closeSet.Add(crtNode);

                if (crtNode == targetNode)
                {
                    //sw.Stop();
                    //print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;

                    break;
                }

                foreach (Node neighbour in _grid.GetNeighbours(crtNode))
                {
                    if (!neighbour.walkable || closeSet.Contains(neighbour)) continue;

                    /*if (crtNode.gridX != neighbour.gridX && crtNode.gridY != neighbour.gridY)
                    {
                        if (!Grid.inst.NodeFromGridPoint(neighbour.gridX, crtNode.gridY).walkable
                            || !Grid.inst.NodeFromGridPoint(crtNode.gridX, neighbour.gridY).walkable)
                            continue;
                    }*/

                    int newMovementCostToNeighbour = crtNode.gCost + GetDistance(crtNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = crtNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else openSet.UpdateItem(neighbour);
                    }
                }
            }
        }

        if (pathSuccess)
        {
            wayPoints = RetracePath(startNode, targetNode);
            pathSuccess = wayPoints.Length > 0;
        }
        callback(new PathResult(wayPoints, pathSuccess, request.callback));

        _grid.ResetGrid();
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node crtNode = endNode;
        path.Add(endNode);
        while (crtNode != startNode)
        {
            path.Add(crtNode);
            crtNode = crtNode.parent;
        }

        Vector3[] wayPoints = SimplifyPath(path);
        //wayPoints.Reverse();

        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            //Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            //if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].worldPosition);
                //directionOld = directionNew;
            }
        }
        wayPoints.Reverse();
        return wayPoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
