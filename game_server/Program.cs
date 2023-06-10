using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;

using System.Text.Json;

using GameServer;
using GameServer.Hosts;
using GameServer.Models;

class Program
{

    static void Main(string[] args)
    {
        new UDPServer(10).start();
    }
}
