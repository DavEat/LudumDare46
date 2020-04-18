using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridEntity : MonoBehaviour
{
    protected Transform m_transform = null;
    public Node crtNode = null;

    protected virtual void Start()
    {
        m_transform = GetComponent<Transform>();
        crtNode = Grid.inst.NodeFromWorldPoint(m_transform.position);
    }
    public abstract void RevertTurn(ITurnAction action);
}
