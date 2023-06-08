using System.Net;

namespace GameServer
{

    class Player
    {
        private int connected_hosts;
        private int max_hosts;

        private List<PlayerInstance> hostList;
        private List<IPEndPoint> knowHosts;

        public Player(int max_hosts)
        {
            this.max_hosts = max_hosts;
            this.connected_hosts = 0;

            hostList = new List<PlayerInstance>();
            knowHosts = new List<IPEndPoint>();
        }

        public bool tryConnect(IPEndPoint ie, PlayerModel pm)
        {
            if (this.connected_hosts == this.max_hosts) return false;

            foreach (IPEndPoint h in knowHosts)
                if (ie == h) return true;

            hostList.Add(new PlayerInstance(ie, connected_hosts, pm));
            knowHosts.Add(ie);

            this.connected_hosts++;

            return true;
        }

        public bool doUpdate(IPEndPoint ie, PlayerModel pm)
        {
            foreach (PlayerInstance p in hostList)
            {
                if (p.getEndPoint() == ie)
                {
                    p.setPm(pm);
                }
            }

        }


    }
}