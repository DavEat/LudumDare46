using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public delegate void EndTurn();
    public EndTurn endTurn;

    //float m_time;
    //[SerializeField] float m_holdTime = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            #if !UNITY_WEBGL
            if (SceneManager.GetActiveScene().buildIndex == 0)
                Application.Quit();
            else
            #endif
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //m_time = Time.time + m_holdTime;
            SoundManager.inst.PlayRevers();
        }
        else if (Input.GetKeyUp(KeyCode.Backspace))
        {
            SoundManager.inst.StopRevers();
        }
        else if (Input.GetKey(KeyCode.Backspace))
        {
            //if (m_time < Time.time)
            if (!SoundManager.inst.ReversIsPlaying())
            {
                Score.inst.RevertScore();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void CallEndTurn()
    {
        if (endTurn != null)
            endTurn.Invoke();
        BackInTimeManager.inst.StoreCrtTurnActions();
    }
}