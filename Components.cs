using Godot;
using System.Collections.Generic;

namespace Comp
{
    class ActorDef
    {
        public global::ActorDef def;
    }

    class Player
    {
        public Vector2 linear_vel = Vector2.Zero;
        public float onair_time = 0;
        public bool on_floor = false;
        public float shoot_time = 99999; //time since last shot

        public string anim = "";
    }

    class Monster
    {

    }

    class Collectible
    {

    }

    class Projectile
    {

    }

    class GodotSceneSingleton
    {

    }

    class BulletImpactCollector
    {

    }
}
