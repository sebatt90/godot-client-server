using GameServer.Models;
using GameServer.Game;

namespace GameServer.Hosts
{
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

        public ReqModel toUpdateRequest()
        {
            ReqModel req = new ReqModel
            {
                Id = PlayerId,
                Type = "UPDATE",
                Name = PlayerName,
                pos_x = Position.X,
                pos_y = Position.Y
            };
            return req;
        }
    }
}