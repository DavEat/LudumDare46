using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Pixalate : MonoBehaviour
{
    [SerializeField] Material m_effectMat = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_effectMat);
    }
}
