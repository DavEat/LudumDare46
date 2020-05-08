using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SwipeInput : MonoBehaviour
{
    public Vector3 angle = Vector3.zero;
    public float minDst = 50;
    [Header("Input")]
    public UnityEvent up, down, right, left;
    public UnityEvent click;

    Vector2 m_lastDeltaPosition = Vector2.zero;

    bool m_beganClick = false;

    void Start()
    {
        //GameManager.inst.initiateGame += () => { m_beganClick = false; };
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                m_lastDeltaPosition = Quaternion.Euler(angle) * Input.GetTouch(0).deltaPosition;

                if (m_lastDeltaPosition.sqrMagnitude < minDst)
                {
                    if (m_beganClick)
                    {
                        m_beganClick = false;
                        click.Invoke();
                    }
                    return;
                }
                m_beganClick = false;

                if (Mathf.Abs(m_lastDeltaPosition.x) > Mathf.Abs(m_lastDeltaPosition.y))
                {
                    if (m_lastDeltaPosition.x > 0)
                        right.Invoke();
                    else left.Invoke();
                }
                else
                {
                    if (m_lastDeltaPosition.y > 0)
                        up.Invoke();
                    else down.Invoke();
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                m_beganClick = true;
            }
        }
    }
}