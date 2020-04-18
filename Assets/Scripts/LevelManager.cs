using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<WaterBubble> m_waters = new List<WaterBubble>();
    [SerializeField] List<Door> m_doors = new List<Door>();

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