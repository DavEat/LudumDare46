using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : GridEntity
{
    int sceneOffset = 0;

    public override void RevertTurn(ITurnAction action) { }

    public void Active()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1 + sceneOffset;
        if (index >= SceneManager.sceneCountInBuildSettings) index = 0;
        SceneManager.LoadScene(index);
    }
}
