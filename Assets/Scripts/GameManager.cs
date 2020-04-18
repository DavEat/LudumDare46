using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public delegate void EndTurn();
    public EndTurn endTurn;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            GoBackInTime();
        }
    }

    public void CallEndTurn()
    {
        endTurn.Invoke();
        StoreCrtTurnActions();
    }

    //List<List<TurnAction>> m_turnsActions = new List<List<TurnAction>>();
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
            foreach (ITurnAction action in m_crtTurnActions)
            {
                action.Revert();
            }
            m_turnsActions.RemoveFirst();

            m_crtTurnActions.Clear();
        }
        else m_crtTurnActions = new List<ITurnAction>();
    }
    void StoreCrtTurnActions()
    {
        List<ITurnAction> actions = new List<ITurnAction>();

        foreach (ITurnAction action in m_crtTurnActions)
        {
            actions.Add(action);
        }

        m_turnsActions.Add(actions);
    }
}


public class PrependList<T>
{
    Collection first = null;

    public void Add(T t)
    {
        first = new Collection(first, t);
    }
    public T GetFirst()
    {
        return first != null ? first.obj : default(T);
    }
    public void RemoveFirst()
    {
        if (first != null)
            first = first.next;
    }

    public class Collection
    {
        public Collection next;
        public T obj;

        public Collection(Collection next, T obj)
        {
            this.next = next;
            this.obj = obj;
        }
    }
}

public interface ITurnAction
{
    void Revert();
}

public abstract class TurnAction : ITurnAction
{
    protected GridEntity m_entity;

    public TurnAction(GridEntity entity)
    {
        this.m_entity = entity;
    }
    public virtual void Revert()
    {
        m_entity.RevertTurn(this);
    }
}
public class TurnActionMove : TurnAction, ITurnAction
{
    public Node node;

    public TurnActionMove(Node node, GridEntity entity) : base(entity)
    {
        this.node = node;
    }
}
public class TurnActionHit : TurnAction, ITurnAction
{
    public int durability;

    public TurnActionHit(int durability, GridEntity entity) : base (entity)
    {
        this.durability = durability;
    }
}
public class TurnActionMoveHit : TurnAction,  ITurnAction
{
    public Node node;
    public int durability;

    public TurnActionMoveHit(Node node, int durability, GridEntity entity) : base(entity)
    {
        this.node = node;
        this.durability = durability;
    }
}
public class TurnActionSpread : TurnAction, ITurnAction
{
    public TurnActionSpread(GridEntity entity) : base(entity) { }
}
public class TurnActionPickedUp : TurnAction, ITurnAction
{
    public TurnActionPickedUp(GridEntity entity) : base(entity) { }
}