
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
public class TurnActionMoveRotate : TurnActionMove, ITurnAction
{
    public float angleY;

    public TurnActionMoveRotate(Node node, float angleY, GridEntity entity) : base(node, entity)
    {
        this.angleY = angleY;
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
public class TurnActionMoveHit : TurnActionMove, ITurnAction
{
    public int durability;

    public TurnActionMoveHit(Node node, int durability, GridEntity entity) : base(node, entity)
    {
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
public class TurnActionFull : TurnAction, ITurnAction
{
    public TurnActionFull(GridEntity entity) : base(entity) { }
}
public class TurnActionDestroy : TurnAction, ITurnAction
{
    public TurnActionDestroy(GridEntity entity) : base(entity) { }
}