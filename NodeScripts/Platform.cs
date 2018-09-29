using Godot;

namespace Comp
{
    namespace Godot
    {
        class Platform : Node2D
        {
            [Export] public Vector2 motion = Vector2.Zero;
            [Export] public float cycle = 1.0f;

            public float accum;
        }
    }
}
    