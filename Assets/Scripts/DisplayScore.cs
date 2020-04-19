using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] GameObject[] m_waterBubbles = null;

    [SerializeField] Animator m_queenDemandAnim = null;

    void Start()
    {
        int score = Score.inst.score;
        if (score > m_waterBubbles.Length)
            score = m_waterBubbles.Length;

        if (score > 4)
        {
            m_queenDemandAnim.SetBool("Thanks", true);
        }

        for (int i = 0; i < score; i++)
        {
            m_waterBubbles[i].SetActive(true);
        }
    }
}
