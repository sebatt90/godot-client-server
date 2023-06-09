using System.Linq;
using System.Collections.Generic;
using Godot;
using ClientModels;
using System.Text;
using System.Text.Json;

public partial class Client : Node
{

    [Export]
    private string _serverIp;

    [Export]
    private int _port;

    [ExportCategory("Foreign Players")]

    [Export]
    private PackedScene foreignPlayerBase;

    [Export]
    private Node foreignPlayerContainer;

    [ExportCategory("Player info")]
    [Export]
    private Node2D _player;

    private PacketPeerUdp _udp = new PacketPeerUdp();
    private bool _connected = false;


    private PlayerModel plr;
    public override void _Ready()
    {
        // yeah this will be provided by an external node, but so far, we're doing it like this
        _udp.ConnectToHost(_serverIp, _port);
        plr = new PlayerModel
        {
            playerName = PlayerInfo.playerName,
            x_pos = _player.Position.X,
            y_pos = _player.Position.Y
        };
    }

    public override void _Process(double delta)
    {
        if (!_connected)
        {
            // Try to contact server
            plr.x_pos = _player.Position.X;
            plr.y_pos = _player.Position.Y;

            _udp.PutPacket(Encoding.Default.GetBytes(JsonSerializer.Serialize(plr)));
        }
        if (_udp.GetAvailablePacketCount() > 0)
        {
            //GD.Print($"Connected: {_udp.GetPacket().GetStringFromUtf8()}");

            List<string> tList = _udp.GetPacket().GetStringFromUtf8().Split(';').ToList<string>();

            if (tList != null)
                updatePlayers(tList);
        }
    }

    private void updatePlayers(List<string> pList)
    {
        foreach (string json in pList)
        {
            GD.Print(json);
            PlayerModel pm = JsonSerializer.Deserialize<PlayerModel>(json);
            Node2D foreignPlayer;
            // check if already exists
            for (int i = 0; i < foreignPlayerContainer.GetChildCount(); i++)
            {
                foreignPlayer = foreignPlayerContainer.GetChild<ForeignPlayer>(i);

                if (((ForeignPlayer)foreignPlayer).id == pm.id)
                {
                    foreignPlayer.Position = new Vector2(pm.x_pos, pm.y_pos);

                    return;
                }
            }


            foreignPlayer = foreignPlayerBase.Instantiate<Node2D>();

            ((ForeignPlayer)foreignPlayer).name = pm.playerName;
            ((ForeignPlayer)foreignPlayer).id = pm.id;

            foreignPlayer.Position = new Vector2(pm.x_pos, pm.y_pos);

            foreignPlayerContainer.AddChild(foreignPlayer);
        }
    }
}
