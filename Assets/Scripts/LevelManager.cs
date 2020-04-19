using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    List<Door> m_doors = new List<Door>();
    List<WaterBubble> m_waters = new List<WaterBubble>();

    Dictionary<Node, Rock> rocks = new Dictionary<Node, Rock>();
    Dictionary<Node, Pit> pits = new Dictionary<Node, Pit>();

    [SerializeField] Transform[] rockParents = null;
    [SerializeField] Transform pitParents = null;
    [SerializeField] Transform doorsParent = null;
    [SerializeField] Transform waterParent = null;

    void Start()
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        for (int e = 0; e < rockParents.Length; e++)
        {
            Rock[] r = rockParents[e].GetComponentsInChildren<Rock>();
            for (int i = 0; i < r.Length; i++)
            {
                rocks.Add(r[i].crtNode, r[i]);
            }
        }

        Pit[] p = pitParents.GetComponentsInChildren<Pit>();
        for (int i = 0; i < p.Length; i++)
        {
            pits.Add(p[i].crtNode, p[i]);
        }

        doorsParent.GetComponentsInChildren<Door>(m_doors);
        waterParent.GetComponentsInChildren<WaterBubble>(m_waters);
    }

    public Rock IsARock(Node node)
    {
        Rock rock = null;
        rocks.TryGetValue(node, out rock);

        if (rock != null && (rock.onAPit || !rock.gameObject.activeSelf))
        {
            rock = null;
        }

        return rock;
    }
    public void UpdateRockNode(Node previousNode, Rock rock, Node newNode)
    {
        if (previousNode != null)
            rocks.Remove(previousNode);
        if (newNode != null)
            rocks.Add(newNode, rock);
    }

    public Pit IsAPit(Node node)
    {
        Pit pit = null;
        pits.TryGetValue(node, out pit);

        if (pit != null && (pit.full || !pit.gameObject.activeSelf))
        {
            pit = null;
        }

        return pit;
    }
    public void UpdatePitNode(Node previousNode, Pit pit, Node newNode)
    {
        if (previousNode != null)
            pits.Remove(previousNode);
        if (newNode != null)
            pits.Add(newNode, pit);
    }

    public Door IsADoor(Node node)
    {
        foreach (Door door in m_doors)
        {
            if (door.crtNode == node)
            {
                return door;
            }
        }
        return null;
    }
    public WaterBubble IsAWaterBubble(Node node)
    {
        foreach (WaterBubble water in m_waters)
        {
            if (water.crtNode == node && water.gameObject.activeSelf)
            {
                return water;
            }
        }
        return null;
    }
}