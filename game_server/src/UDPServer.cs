using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

using System.Text.Json;

using GameServer;
using GameServer.Hosts;
using GameServer.Models;

namespace GameServer
{
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

        public void start()
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
                    switch (req.Type)
                    {
                        case "JOIN":
                            {
                                int id = hostHandler.addNewHost(ep, req);

                                Console.WriteLine((id == -1) ? $"{ep.ToString()} has tried to join, but failed" : $"{req.Name} ({ep.ToString()}) has joined");
                                send(id.ToString());
                                break;
                            }
                        case "UPDATE":
                            {
                                List<ReqModel> resList = hostHandler.updatePlayers(ep, req);

                                string res = "";

                                for (int i = 0; i < resList.Count; i++)
                                {
                                    res += (JsonSerializer.Serialize(resList[i]) + (i == (resList.Count - 1) ? "" : ";"));
                                }

                                send(res);
                                break;
                            }
                        case "DISCONNECT":
                            {
                                // TODO
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

        private void send(string res)
        {
            byte[] resBytes = Encoding.ASCII.GetBytes(res);
            udpClient.Send(resBytes, resBytes.Length, ep);
        }
    }
}