using GameServer.Game;
using GameServer.Models;

namespace GameServer.Hosts;

public class PlayerInstance
{
    // vars
    public string PlayerName { get; }

    public int PlayerId { get; }

    public Vector2 Position { get; set; }

    public PlayerInstance(string playerName, int playerId)
    {
        this.PlayerName = playerName;
        this.PlayerId = playerId;
    }

    public ReqModel ToUpdateRequest()
    {
        ReqModel req = new()
        {
            Id = PlayerId,
            Type = "UPDATE",
            Name = PlayerName,
            Pos_x = Position.X,
            Pos_y = Position.Y
        };
        return req;
    }
}