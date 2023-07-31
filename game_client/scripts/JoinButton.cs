using System.Linq;
using Godot;

public partial class JoinButton : Button
{
    [Export]
    private LineEdit nameLabel;
    [Export]
    private LineEdit iplabel;

    public void _OnPressed()
    {
        PlayerInfo.playerName = nameLabel.Text ?? "playerx";

        PlayerInfo.ip = iplabel.Text.Split(":").ToArray()[0];
        PlayerInfo.port = iplabel.Text.Split(":").ToArray()[1].ToInt();

        GetTree().ChangeSceneToFile("res://world.tscn");
    }
}
