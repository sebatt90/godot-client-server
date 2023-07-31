using GameServer.Hosts;
using GameServer.Models;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace GameServer;

public class UDPServer
{
    private UdpClient udpClient;
    private IPEndPoint ep;

    private string data;

    private HostHandler hostHandler;

    public UDPServer(int max_hosts)
    {
        // Setup
        udpClient = new UdpClient(ServerSettings.port);
        ep = new IPEndPoint(IPAddress.Any, 0);
        data = "";

        hostHandler = new HostHandler(max_hosts);
    }

    public void Start()
    {
        Console.WriteLine("Godot: Game + Server's server started on Port " + ServerSettings.port);

        while (true)
        {
            byte[] receivedData = udpClient.Receive(ref ep);
            data = Encoding.ASCII.GetString(receivedData);

            try
            {
                ReqModel req = JsonSerializer.Deserialize<ReqModel>(data);

                // check request type
                switch (req?.Type)
                {
                    case "JOIN":
                        {
                            int id = hostHandler.addNewHost(ep, req);

                            if (id == -1)
                                break;

                            ReqModel res = new()
                            {
                                Type = "PLAYERJOIN",
                                Name = req.Name,
                                Id = id,
                            };

                            Broadcast(JsonSerializer.Serialize(res));

                            Console.WriteLine((id == -1) ? $"{ep} has tried to join, but failed" : $"{req.Name} ({ep}) has joined");
                            Send(id.ToString());

                            break;
                        }
                    case "UPDATE":
                        {
                            List<ReqModel> resList = hostHandler.UpdatePlayers(ep, req);

                            string res = "";

                            for (int i = 0; i < resList.Count; i++)
                            {
                                res += (JsonSerializer.Serialize(resList[i]) + (i == (resList.Count - 1) ? "" : ";"));
                            }

                            Send(res);
                            break;
                        }
                    case "DISCONNECT":
                        {
                            int host_id = hostHandler.RemoveHostByEndPoint(ep);

                            ReqModel res = new()
                            {
                                Type = "PLAYERDISCONNECT",
                                Id = host_id,
                            };

                            Broadcast(JsonSerializer.Serialize(res));

                            break;
                        }
                    default:
                        {
                            // TODO
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private void Send(string res)
    {
        byte[] resBytes = Encoding.ASCII.GetBytes(res);
        udpClient.Send(resBytes, resBytes.Length, ep);
    }

    private void Broadcast(string res)
    {
        byte[] resBytes = Encoding.ASCII.GetBytes(res);

        foreach (IPEndPoint Ep in hostHandler.Hosts.Keys)
        {
            udpClient.Send(resBytes, resBytes.Length, Ep);
        }
    }
}