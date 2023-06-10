using GameServer.Models;
using GameServer.Game;

namespace GameServer.Hosts
{
    public class PlayerInstance
    {
        // vars
        private string playerName;
        private int playerId;

        public Vector2 Position { get; set; }

        public PlayerInstance(string playerName, int playerId)
        {
            this.playerName = playerName;
            this.playerId = playerId;
        }

        public ReqModel toUpdateRequest()
        {
            ReqModel req = new ReqModel
            {
                Id = playerId,
                Type = "UPDATE",
                pos_x = Position.X,
                pos_y = Position.Y
            };
            return req;
        }
    }
}