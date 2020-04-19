using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : GridEntity
{
    public override void RevertTurn(ITurnAction action) { }

    public void Active()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        if (index >= SceneManager.sceneCountInBuildSettings) index = 1;
        SceneManager.LoadScene(index);
    }
}
