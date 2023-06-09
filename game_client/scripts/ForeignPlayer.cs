using Godot;
using System;

public partial class ForeignPlayer : Node2D
{
    public int id;
    public String name;

    private Label tagName;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        tagName = GetChild<Label>(1);

        tagName.Text = name;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
