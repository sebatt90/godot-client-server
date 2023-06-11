using System.Linq;
using System.Runtime.Serialization;
using Godot;
using System;

public partial class JoinButton : Button
{
    [Export]
    private LineEdit nameLabel;
    [Export]
    private LineEdit iplabel;

    public void _OnPressed()
    {
        PlayerInfo.playerName = (nameLabel.Text != null) ? nameLabel.Text : "playerx";

        PlayerInfo.ip = iplabel.Text.Split(":").ToArray()[0];
        PlayerInfo.port = iplabel.Text.Split(":").ToArray()[1].ToInt();

        GetTree().ChangeSceneToFile("res://world.tscn");
    }
}
