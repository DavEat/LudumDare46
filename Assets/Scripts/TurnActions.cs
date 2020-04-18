
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

    public TurnActionHit(int durability, GridEntity entity) : base(entity)
    {
        this.durability = durability;
    }
}
public class TurnActionMoveHit : TurnAction, ITurnAction
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