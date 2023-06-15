using GameServer.Models;
using GameServer.Game;

namespace GameServer.Hosts
{
    public class PlayerInstance
    {
        // vars
        public string PlayerName { get; }
        private int playerId;

        public Vector2 Position { get; set; }

        public PlayerInstance(string playerName, int playerId)
        {
            this.PlayerName = playerName;
            this.playerId = playerId;
        }

        public ReqModel toUpdateRequest()
        {
            ReqModel req = new ReqModel
            {
                Id = playerId,
                Type = "UPDATE",
                Name = PlayerName,
                pos_x = Position.X,
                pos_y = Position.Y
            };
            return req;
        }
    }
}