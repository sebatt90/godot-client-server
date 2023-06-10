using System.Runtime.Serialization;
using Godot;
using System;

public partial class JoinButton : Button
{
    [Export]
    private LineEdit nameLabel;

    public void _OnPressed()
    {
        PlayerInfo.playerName = (nameLabel.Text != null) ? nameLabel.Text : "playerx";
        GetTree().ChangeSceneToFile("res://world.tscn");
    }
}
