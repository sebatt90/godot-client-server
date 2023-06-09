using System.Text.Json;
using System.Net;

namespace GameServer
{

    class Player
    {
        private int connected_hosts;
        private int max_hosts;

        public List<PlayerInstance> hostList;
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

            foreach (PlayerInstance pi in hostList)
            {
                if (pi.Pm.playerName == pm.playerName)
                    return true;
            }

            PlayerInstance newInst = new PlayerInstance(ie, connected_hosts.ToString(), pm);

            newInst.Pm.id = this.connected_hosts;

            hostList.Add(newInst);
            knowHosts.Add(ie);

            this.connected_hosts++;

            return true;
        }

        public void doUpdate(IPEndPoint ie, PlayerModel pm)
        {
            foreach (PlayerInstance p in hostList)
            {
                if (p.Pm.playerName == pm.playerName)
                {
                    p.Pm = pm;
                }
            }
        }

        public string getOtherPlayerJSON(PlayerModel pm)
        {
            string s = "";

            foreach (PlayerInstance p in hostList)
            {
                if (p.Pm.playerName != pm.playerName)
                {
                    string comma = ";";

                    if (s == "") comma = "";

                    s += (comma + JsonSerializer.Serialize(p.Pm));
                }
            }

            return s;
        }




    }
}