using System.Net;
using GameServer.Models;
using GameServer.Game;

namespace GameServer.Hosts
{
    public class HostHandler
    {
        public readonly int MAX_PLAYERS;

        private int host_count;

        private Dictionary<IPEndPoint, PlayerInstance> hosts;

        public HostHandler(int max_players)
        {
            MAX_PLAYERS = max_players;
            host_count = 0;
            hosts = new Dictionary<IPEndPoint, PlayerInstance>();
        }

        public int addNewHost(IPEndPoint ep, ReqModel req)
        {
            // TODO: this should kick you out the server
            if (host_count >= MAX_PLAYERS)
                return -1;

            PlayerInstance newInst = new PlayerInstance(req.Name, host_count);
            newInst.Position = new Vector2(req.pos_x, req.pos_y);

            hosts.Add(ep, newInst);

            host_count++;

            return host_count - 1;

        }

        public void removeHostByEndPoint(IPEndPoint ep, ReqModel req)
        {
            try
            {
                Console.WriteLine($"{hosts[ep].PlayerName} ({ep}) has disconnected");
                hosts.Remove(ep);
                host_count--;
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e);
            }
        }

        public List<ReqModel> updatePlayers(IPEndPoint ep, ReqModel req)
        {
            hosts[ep].Position = new Vector2(req.pos_x, req.pos_y);

            List<ReqModel> list = new List<ReqModel>();
            foreach (PlayerInstance pInst in hosts.Values)
            {
                list.Add(pInst.toUpdateRequest());
            }
            return list;
        }


    }
}