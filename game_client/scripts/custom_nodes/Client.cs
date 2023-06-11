using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
using ClientModels;
using System.Text;
using System.Text.Json;

public partial class Client : Node
{

    private int client_id;

    [ExportCategory("Foreign Players")]

    [Export]
    private PackedScene foreignPlayerBase;

    [Export]
    private Node PlayersContainer;

    [ExportCategory("Player info")]
    [Export]
    private PackedScene player;

    private PacketPeerUdp UDP = new PacketPeerUdp();
    private bool connected = false;
    private string res;

    private Node2D playerInst;

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
        UDP.ConnectToHost((PlayerInfo.ip != null) ? PlayerInfo.ip : "127.0.0.1", PlayerInfo.port);

        // send connection packet
        req = new ReqModel()
        {
            Type = "JOIN",
            Name = PlayerInfo.playerName
        };
        GD.Print("Trying to connect...");
        send(JsonSerializer.Serialize(req));


    }

    public override void _Process(double delta)
    {
        // if connected
        switch (connected)
        {
            case true:

                update();
                break;
            case false:
                join();
                break;
        }

    }

    private void join()
    {
        if (UDP.GetAvailablePacketCount() > 0)
        {
            res = response();
            if (res.ToInt() != -1)
            {
                GD.Print("Connected successfully!");
                this.client_id = res.ToInt();
                this.connected = true;

                return;
            }

            GD.Print("Couldn't connect to server!");
        }
    }

    private void update()
    {
        req = new ReqModel()
        {
            Type = "UPDATE",
        };

        req.pos_x = (playerInst == null) ? 0 : playerInst.Position.X;
        req.pos_y = (playerInst == null) ? 0 : playerInst.Position.Y;

        send(JsonSerializer.Serialize(req));
        // Try to contact server
        if (UDP.GetAvailablePacketCount() > 0)
        {
            //GD.Print($"Connected: {_udp.GetPacket().GetStringFromUtf8()}");
            List<string> list = response().Split(";").ToList<string>();

            if (list != null)
                updateAllClients(list);

        }
    }

    // this part should be common to all games, I think
    private void updateAllClients(List<string> pList)
    {
        Node2D obj;


        for (int i = 0; i < pList.Count; i++)
        {
            ReqModel req = JsonSerializer.Deserialize<ReqModel>(pList[i]);


            obj = PlayersContainer.GetChildOrNull<Node2D>(req.Id);

            if (obj == null)
            {
                obj = (client_id == req.Id) ? player.Instantiate<Node2D>() : instForeignPlayer(req.Name);

                if (client_id == req.Id)
                    playerInst = obj;
                // NOTE: perhaps this too could be a signal
                obj.Position = new Vector2(req.pos_x, req.pos_y);

                PlayersContainer.AddChild(obj);

                continue;
            }


            if (client_id != req.Id)
                // update positon
                // NOTE: perhaps this could be a signal
                obj.Position = new Vector2(req.pos_x, req.pos_y);

        }
    }

    private Node2D instForeignPlayer(string name)
    {
        ForeignPlayer fPlr = foreignPlayerBase.Instantiate<ForeignPlayer>();
        fPlr.name = name;

        return (Node2D)fPlr;
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
