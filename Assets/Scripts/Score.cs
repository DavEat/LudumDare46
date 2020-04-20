using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Score : Singleton<Score>
{
    int m_score;
    public int score { get { return m_score; } }

    bool m_scoreAdded = false;

    void Start()
    {
        Debug.Log("Start");
    }

    void OnEnable()
    {
        Debug.Log("Enable");
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_scoreAdded = false;

        if (scene.buildIndex == 0)
        {
            m_score = 0;
            Score[] scores = FindObjectsOfType<Score>();
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] != Score.inst)
                    Destroy(scores[i].gameObject);
            }
        }
    }

    public void AddScore()
    {
        m_score++;
        m_scoreAdded = true;
    }
    public void RevertScore()
    {
        if (m_scoreAdded)
        {
            m_score--;
            m_scoreAdded = false;
        }
    }
}