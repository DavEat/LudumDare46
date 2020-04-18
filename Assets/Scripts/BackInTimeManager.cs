using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackInTimeManager : Singleton<BackInTimeManager>
{
    PrependList<List<ITurnAction>> m_turnsActions = new PrependList<List<ITurnAction>>();
    List<ITurnAction> m_crtTurnActions = new List<ITurnAction>();

    public void AddAction(ITurnAction action)
    {
        m_crtTurnActions.Add(action);
    }
    public void GoBackInTime()
    {
        m_crtTurnActions = m_turnsActions.GetFirst();

        if (m_crtTurnActions != null)
        {
            foreach (ITurnAction action in m_crtTurnActions.ToList())
            {
                action.Revert();
            }
            m_crtTurnActions.Clear();
            m_turnsActions.RemoveFirst();
        }
        else
        {
            Debug.Log("no more return possible");
            m_crtTurnActions = new List<ITurnAction>();
        }
    }
    public void StoreCrtTurnActions()
    {
        List<ITurnAction> actions = new List<ITurnAction>();

        foreach (ITurnAction action in m_crtTurnActions)
        {
            actions.Add(action);
        }
        m_crtTurnActions.Clear();
        m_turnsActions.Add(actions);
    }
}
