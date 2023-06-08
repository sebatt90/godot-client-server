using Godot;
using ClientModels;
using System.Text;
using System.Text.Json;

public partial class Client : Node
{
    [Export]
    private string _name;

    [Export]
    private string _serverIp;

    [Export]
    private int _port;

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
            playerName = _name,
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
            GD.Print($"Connected: {_udp.GetPacket().GetStringFromUtf8()}");
            _connected = true;
        }
    }
}
