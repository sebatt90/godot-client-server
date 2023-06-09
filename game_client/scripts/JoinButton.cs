using System.Runtime.Serialization;
using Godot;
using System;

public partial class JoinButton : Button
{
    [Export]
    private TextEdit nameLabel;

    public void _OnPressed()
    {
        PlayerInfo.playerName = (nameLabel.Text != null) ? nameLabel.Text : "nullo";
        GetTree().ChangeSceneToFile("res://world.tscn");
    }
}
