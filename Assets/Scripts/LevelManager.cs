using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    List<Door> m_doors = new List<Door>();
    List<WaterBubble> m_waters = new List<WaterBubble>();

    Dictionary<Node, Rock> m_rocks = new Dictionary<Node, Rock>();
    Dictionary<Node, Pit> m_pits = new Dictionary<Node, Pit>();
    Dictionary<Node, Cactus> m_cactus = new Dictionary<Node, Cactus>();

    [SerializeField] Transform[] rockParents = null;
    [SerializeField] Transform pitParents = null;
    [SerializeField] Transform m_cactusParents = null;
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
                m_rocks.Add(r[i].crtNode, r[i]);
            }
        }
        if (pitParents != null)
        {
            Pit[] p = pitParents.GetComponentsInChildren<Pit>();
            for (int i = 0; i < p.Length; i++)
            {
                m_pits.Add(p[i].crtNode, p[i]);
            }
        }
        if (m_cactusParents != null)
        {
            Cactus[] c = m_cactusParents.GetComponentsInChildren<Cactus>();
            for (int i = 0; i < c.Length; i++)
            {
                m_cactus.Add(c[i].crtNode, c[i]);
            }
        }

        if (doorsParent != null) doorsParent.GetComponentsInChildren<Door>(m_doors);
        if (waterParent != null) waterParent.GetComponentsInChildren<WaterBubble>(m_waters);
    }

    public Rock IsARock(Node node)
    {
        Rock rock = null;
        m_rocks.TryGetValue(node, out rock);

        if (rock != null && (rock.onAPit || !rock.gameObject.activeSelf))
        {
            rock = null;
        }

        return rock;
    }
    public void UpdateRockNode(Node previousNode, Rock rock, Node newNode)
    {
        if (previousNode != null)
            m_rocks.Remove(previousNode);
        if (newNode != null)
            m_rocks.Add(newNode, rock);
    }

    public Pit IsAPit(Node node)
    {
        Pit pit = null;
        m_pits.TryGetValue(node, out pit);

        if (pit != null && (pit.full || !pit.gameObject.activeSelf))
        {
            pit = null;
        }

        return pit;
    }
    public void UpdatePitNode(Node previousNode, Pit pit, Node newNode)
    {
        if (previousNode != null)
            m_pits.Remove(previousNode);
        if (newNode != null)
            m_pits.Add(newNode, pit);
    }
    public Cactus IsACactus(Node node)
    {
        Cactus cactus = null;
        m_cactus.TryGetValue(node, out cactus);

        if (cactus != null && !cactus.gameObject.activeSelf)
        {
            cactus = null;
        }

        return cactus;
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