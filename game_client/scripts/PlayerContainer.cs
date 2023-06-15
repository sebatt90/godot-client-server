using System.Collections.Generic;
using Godot;
using System;

public partial class PlayerContainer : Node2D
{
    public Node2D getPlayerAtOrNull(int idx)
    {
        foreach (Node2D n in GetChildren())
            if ((n is Player && ((Player)n).player_id == idx) || (n is ForeignPlayer && ((ForeignPlayer)n).id == idx))
                return n;
        return null;
    }

}
