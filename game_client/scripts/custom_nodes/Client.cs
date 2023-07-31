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
    private PlayerContainer playersContainer;

    [ExportCategory("Player info")]
    [Export]
    private PackedScene player;

    private PacketPeerUdp UDP = new PacketPeerUdp();
    private bool connected = false;
    private string res;

    private Player playerInst;

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

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            if (connected)
            {
                connected = false;
                // send disconnect packet
                req = new ReqModel
                {
                    Type = "DISCONNECT",
                };
                send(JsonSerializer.Serialize(req));
                UDP.Close();
                GD.Print("Connection closed!");
            }
            GetTree().Quit(); // default behavior
        }
    }

    // server functions

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

                // function to init player
                playerInit();

                return;
            }

            GD.Print("Couldn't connect to server!");
        }
    }

    private void playerInit()
    {
        playerInst = (Player)player.Instantiate<Node2D>();
        playerInst.Id = client_id;
        playerInst.Position = Vector2.Zero;

        playersContainer.AddChild(playerInst);

    }

    private void playerJoinEvent(string name, int id)
    {
        ForeignPlayer fp = (ForeignPlayer)instForeignPlayer(name, id);
        playersContainer.AddChild(fp);
    }

    private void playerDisconnectEvent(int disconnect_id)
    {
        foreach (Node2D plr in playersContainer.GetChildren())
            if (plr is ForeignPlayer player && player.id == disconnect_id)
                playersContainer.RemoveChild(plr);
    }

    private void update()
    {
        req = new ReqModel
        {
            Type = "UPDATE",
            pos_x = (playerInst == null) ? 0 : playerInst.Position.X,
            pos_y = (playerInst == null) ? 0 : playerInst.Position.Y
        };

        send(JsonSerializer.Serialize(req));

        eventType();
    }

    private void eventType()
    {
        if (UDP.GetAvailablePacketCount() > 0)
        {
            string res = response();


            List<string> list = res.Split(";").ToList<string>();

            if (list == null) return;

            ReqModel req = JsonSerializer.Deserialize<ReqModel>(list[0]);

            switch (req.Type)
            {
                case "UPDATE":
                    {
                        updateAllClients(list);
                        break;
                    }
                case "PLAYERJOIN":
                    {
                        playerJoinEvent(req.Name, req.Id);
                        break;
                    }
                case "PLAYERDISCONNECT":
                    {
                        playerDisconnectEvent(req.Id);
                        break;
                    }
            }
        }
    }

    // this part should be common to all games, I think
    private void updateAllClients(List<string> pList)
    {
        Node2D obj = null;

        for (int i = 0; i < pList.Count; i++)
        {
            ReqModel req = JsonSerializer.Deserialize<ReqModel>(pList[i]);

            obj = playersContainer.getPlayerAtOrNull(req.Id);

            if (obj == null)
            {
                playerJoinEvent(req.Name, req.Id);
                continue;
            }

            if (client_id != req.Id)
                // update positon
                // NOTE: perhaps this could be a signal
                obj.Position = new Vector2(req.pos_x, req.pos_y);

            // reset object to null
            obj = null;
        }
    }

    private Node2D instForeignPlayer(string name, int id)
    {
        ForeignPlayer fPlr = foreignPlayerBase.Instantiate<ForeignPlayer>();
        fPlr.name = name;
        fPlr.id = id;

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
