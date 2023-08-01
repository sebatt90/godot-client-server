using System.Linq;
using System.Collections.Generic;
using Godot;
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

    private PacketPeerUdp UDP = new();
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
        UDP.ConnectToHost(PlayerInfo.ip ?? "127.0.0.1", PlayerInfo.port);

        // send connection packet
        req = new ReqModel()
        {
            Type = "JOIN",
            Name = PlayerInfo.playerName
        };
        GD.Print("Trying to connect...");
        Send(JsonSerializer.Serialize(req));


    }

    public override void _Process(double delta)
    {
        // if connected
        switch (connected)
        {
            case true:
                Update();
                break;
            case false:
                Join();
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
                Send(JsonSerializer.Serialize(req));
                UDP.Close();
                GD.Print("Connection closed!");
            }
            GetTree().Quit(); // default behavior
        }
    }

    // server functions

    private void Join()
    {
        if (UDP.GetAvailablePacketCount() > 0)
        {
            res = Response();
            if (res.ToInt() != -1)
            {
                GD.Print("Connected successfully!");
                this.client_id = res.ToInt();
                this.connected = true;

                // function to init player
                PlayerInit();

                return;
            }

            GD.Print("Couldn't connect to server!");
        }
    }

    private void PlayerInit()
    {
        playerInst = (Player)player.Instantiate<Node2D>();
        playerInst.Id = client_id;
        playerInst.Position = Vector2.Zero;

        playersContainer.AddChild(playerInst);

    }

    private void PlayerJoinEvent(string name, int id)
    {
        ForeignPlayer fp = (ForeignPlayer)InstForeignPlayer(name, id);
        playersContainer.AddChild(fp);
    }

    private void PlayerDisconnectEvent(int disconnect_id)
    {
        foreach (Node2D plr in playersContainer.GetChildren())
            if (plr is ForeignPlayer player && player.id == disconnect_id)
                playersContainer.RemoveChild(plr);
    }

    private void Update()
    {
        req = new ReqModel
        {
            Type = "UPDATE",
            pos_x = (playerInst == null) ? 0 : playerInst.Position.X,
            pos_y = (playerInst == null) ? 0 : playerInst.Position.Y
        };

        Send(JsonSerializer.Serialize(req));

        EventType();
    }

    private void EventType()
    {
        if (UDP.GetAvailablePacketCount() > 0)
        {
            string res = Response();


            List<string> list = res.Split(";").ToList<string>();

            if (list == null) return;

            ReqModel req = JsonSerializer.Deserialize<ReqModel>(list[0]);

            switch (req.Type)
            {
                case "UPDATE":
                    {
                        UpdateAllClients(list);
                        break;
                    }
                case "PLAYERJOIN":
                    {
                        PlayerJoinEvent(req.Name, req.Id);
                        break;
                    }
                case "PLAYERDISCONNECT":
                    {
                        PlayerDisconnectEvent(req.Id);
                        break;
                    }
            }
        }
    }

    // this part should be common to all games, I think
    private void UpdateAllClients(List<string> pList)
    {
        Node2D obj;

        for (int i = 0; i < pList.Count; i++)
        {
            ReqModel req = JsonSerializer.Deserialize<ReqModel>(pList[i]);

            obj = playersContainer.GetPlayerAtOrNull(req.Id);

            if (obj == null)
            {
                PlayerJoinEvent(req.Name, req.Id);
                continue;
            }

            if (client_id != req.Id)
                // update positon
                // NOTE: perhaps this could be a signal
                obj.Position = new Vector2(req.pos_x, req.pos_y);
        }
    }

    private Node2D InstForeignPlayer(string name, int id)
    {
        ForeignPlayer fPlr = foreignPlayerBase.Instantiate<ForeignPlayer>();
        fPlr.name = name;
        fPlr.id = id;

        return fPlr;
    }

    private void Send(string req) => UDP.PutPacket(Encoding.Default.GetBytes(req));

    private string Response() => UDP.GetPacket().GetStringFromUtf8();

    // The functions listed below are very much game specific
}
