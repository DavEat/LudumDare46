using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rock", menuName = "Data/Rock", order = 1)]
public class RockData : ScriptableObject
{
    public int durability = 1;

    public bool breakable
    {
        get { return durability > 1; }
    }
}