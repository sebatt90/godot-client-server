using Godot;

public partial class PlayerContainer : Node2D
{
    public Node2D getPlayerAtOrNull(int idx)
    {
        foreach (Node2D n in GetChildren())
            if ((n is Player player && player.Id == idx) || (n is ForeignPlayer foreignPlayer && foreignPlayer.id == idx))
                return n;
        return null;
    }

}
