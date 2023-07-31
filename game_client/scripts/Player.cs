using Godot;
using System;

public partial class Player : CharacterBody2D
{

    public int Id;

    [Export]
    private int speed { get; set; } = 400;

    public void GetInput()
    {

        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
        Velocity = inputDirection * speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
    }
}
