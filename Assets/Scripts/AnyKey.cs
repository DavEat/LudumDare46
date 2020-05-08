using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnyKey : MonoBehaviour
{
    private void Update()
    {
        Inputs();
    }
    void Inputs()
    {
        //if (m_animated) return;

        if (Input.GetKeyUp(KeyCode.Backspace))
        {
            //BackInTimeManager.inst.GoBackInTime();
            //return;
            GoNext();
        }

        //if (m_isInAPitOrACactus) return;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Move(Vector2.up);
            GoNext();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //Move(Vector2.down);
            GoNext();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Move(Vector2.right);
            GoNext();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Move(Vector2.left);
            GoNext();
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
        {
            GoNext();
        }
    }

    void GoNext()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        if (index >= SceneManager.sceneCountInBuildSettings) index = 0;
        SceneManager.LoadScene(index);
    }
}
