using System.Net;

namespace GameServer
{
    class PlayerInstance
    {
        private string id;
        private IPEndPoint ie;
        private PlayerModel pm;

        public PlayerInstance(IPEndPoint ie, string id, PlayerModel pm)
        {
            this.ie = ie;
            this.id = id;
            this.pm = pm;
        }

        public PlayerModel Pm
        {
            get { return pm; }
            set { this.pm = value; }
        }

        public IPEndPoint EndPoint
        {
            get { return ie; }
        }
    }
}