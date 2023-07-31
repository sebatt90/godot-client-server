using GameServer.Game;
using GameServer.Models;
using System.Net;

namespace GameServer.Hosts;

public class HostHandler
{
    public readonly int MAX_PLAYERS;

    private int host_count;

    public Dictionary<IPEndPoint, PlayerInstance> Hosts { get; private set; }

    public HostHandler(int max_players)
    {
        MAX_PLAYERS = max_players;
        host_count = 0;
        Hosts = new Dictionary<IPEndPoint, PlayerInstance>();
    }

    public int addNewHost(IPEndPoint ep, ReqModel req)
    {
        // TODO: this should kick you out the server
        if (host_count >= MAX_PLAYERS)
            return -1;

        PlayerInstance newInst = new(req.Name, host_count);
        newInst.Position = new Vector2(req.pos_x, req.pos_y);

        Hosts.Add(ep, newInst);

        host_count++;

        return host_count - 1;
    }

    public int removeHostByEndPoint(IPEndPoint ep)
    {
        int host_id = -1;
        try
        {
            Console.WriteLine($"{Hosts[ep].PlayerName} ({ep}) has disconnected");
            host_id = Hosts[ep].PlayerId;
            Hosts.Remove(ep);
            host_count--;
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e);
        }

        return host_id;
    }

    public List<ReqModel> updatePlayers(IPEndPoint ep, ReqModel req)
    {
        Hosts[ep].Position = new Vector2(req.pos_x, req.pos_y);

        List<ReqModel> list = new();
        foreach (PlayerInstance pInst in Hosts.Values)
        {
            list.Add(pInst.toUpdateRequest());
        }
        return list;
    }
}