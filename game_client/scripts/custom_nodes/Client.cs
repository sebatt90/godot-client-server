using System.Linq;
using System.Collections.Generic;
using Godot;
using ClientModels;
using System.Text;
using System.Text.Json;

public partial class Client : Node
{

    [Export]
    private string serverIp;

    [Export]
    private int port;

    private int client_id;

    [ExportCategory("Foreign Players")]

    [Export]
    private PackedScene foreignPlayerBase;

    [Export]
    private Node foreignPlayerContainer;

    [ExportCategory("Player info")]
    [Export]
    private Node2D player;

    private PacketPeerUdp UDP = new PacketPeerUdp();
    private bool connected = false;
    private string res;

    private ReqModel req;
    public override void _Ready()
    {
        // connecting to the server
        /*
            TEMPORARY
            some useful commands here:
            put packets
            _udp.PutPacket(Encoding.Default.GetBytes(JsonSerializer.Serialize(plr)));

            get response string
            _udp.GetPacket().GetStringFromUtf8()

        */
        UDP.ConnectToHost(serverIp, port);

        // send connection packet
        req = new ReqModel()
        {
            Type = "JOIN",
            Name = PlayerInfo.playerName
        };
        GD.Print("Trying to connect...");
        send(JsonSerializer.Serialize(req));

        if (UDP.GetAvailablePacketCount() > 0)
        {
            res = response();

            if (res.ToInt() != -1)
            {
                this.client_id = res.ToInt();
                this.connected = true;

                GD.Print("Connected successfully!");
                return;
            }

            GD.Print("Couldn't connect to server!");
        }
    }

    public override void _Process(double delta)
    {
        if (connected)
        {
            req = new ReqModel()
            {
                Type = "UPDATE",
                pos_x = 1,
                pos_y = 1
            };
            // Try to contact server
            if (UDP.GetAvailablePacketCount() > 0)
            {
                //GD.Print($"Connected: {_udp.GetPacket().GetStringFromUtf8()}");
            }

        }

    }

    // this part should be common to all games, I think
    private void updateAllClients(List<ReqModel> pList)
    {
        // TODO
    }

    private void send(string req)
    {
        UDP.PutPacket(Encoding.Default.GetBytes(req));
    }

    private string response()
    {
        return UDP.GetPacket().GetStringFromUtf8();
    }

    // The functions listed below are very much game specific
}
