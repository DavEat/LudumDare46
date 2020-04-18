using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public delegate void EndTurn();
    public EndTurn endTurn;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BackInTimeManager.inst.GoBackInTime();
        }
    }

    public void CallEndTurn()
    {
        endTurn.Invoke();
        BackInTimeManager.inst.StoreCrtTurnActions();
    }
}