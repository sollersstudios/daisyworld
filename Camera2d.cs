using System;
using Godot;

public partial class Camera2d : Camera2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GlobalPosition = new Vector2(3000, 2500); // Equivalent to make_current()
        Zoom = new Vector2(0.1f, 0.1f);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    private float speed = 500;
    private Vector2 zoomFactor = new Vector2(0.003f, 0.003f);

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;
        Vector2 velocity = Vector2.Zero;

        if (Input.IsKeyPressed(Key.A))
            velocity.X -= 1;
        if (Input.IsKeyPressed(Key.D))
            velocity.X += 1;
        if (Input.IsKeyPressed(Key.W))
            velocity.Y -= 1;
        if (Input.IsKeyPressed(Key.S))
            velocity.Y += 1;
        if (Input.IsKeyPressed(Key.Q))
            Zoom -= zoomFactor;
        if (Input.IsKeyPressed(Key.E))
            Zoom += zoomFactor;

        if (velocity.Length() > 0)
            velocity = velocity.Normalized() * speed;

        Position += velocity * fDelta;
    }
}
