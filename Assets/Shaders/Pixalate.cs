using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Pixalate : MonoBehaviour
{
    [SerializeField] float m_pixelRation = .3f;
    [SerializeField] Material m_effectMat = null;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_effectMat.SetFloat("_Lines", source.width * m_pixelRation);
        m_effectMat.SetFloat("_Rows", source.height * m_pixelRation);
        Graphics.Blit(source, destination, m_effectMat);
    }
}
