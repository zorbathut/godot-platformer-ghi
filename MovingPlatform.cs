using Godot;

public class MovingPlatform : Node2D
{
    [Export] public Vector2 motion = Vector2.Zero;
    [Export] public float cycle = 1.0f;
}
