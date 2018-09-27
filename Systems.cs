using Godot;
using System.Collections.Generic;

namespace Systems
{
    static class CollectorReset
    {
        public static void Execute()
        {

        }
    }

    static class PlatformMovement
    {
        public static void Execute(Comp.Platform platform, Comp.GodotNode godotNode)
        {
            float delta = 1/60f;

            var mp = godotNode.node as MovingPlatform;

            platform.accum += delta * (1.0f / mp.cycle) * Mathf.Pi * 2;
            platform.accum %= Mathf.Pi * 2;
            var d = Mathf.Sin(platform.accum);
            var xf = Transform2D.Identity;
            xf[2] = mp.motion * d;

            godotNode.node.GetNode<Node2D>("platform").Transform = xf;
        }
    }

    static class PlayerMovement
    {
        readonly static Vector2 GRAVITY_VEC = new Vector2(0, 900);
        readonly static Vector2 FLOOR_NORMAL = new Vector2(0, -1);
        const float MIN_ONAIR_TIME = 0.1f;
        const float WALK_SPEED = 250; // pixels/sec
        const float JUMP_SPEED = 480;
        const float SIDING_CHANGE_SPEED = 10;
        const float BULLET_VELOCITY = 1000;
        const float SHOOT_TIME_SHOW_WEAPON = 0.2f;

        public static void Execute(Comp.Player player, Comp.Position position, Comp.GodotNode godotNode)
        {
            float delta = 1/60f;

            // increment counters
	        player.onair_time += delta;
	        player.shoot_time += delta;        

            /// MOVEMENT ///

	        // Apply Gravity
	        player.linear_vel += delta * GRAVITY_VEC;
	        // Move and Slide
	        player.linear_vel = godotNode.kinematicBody.MoveAndSlide(player.linear_vel, floorNormal: FLOOR_NORMAL);
	        // Detect Floor
	        if (godotNode.kinematicBody.IsOnFloor())
                player.onair_time = 0;

	        player.on_floor = player.onair_time < MIN_ONAIR_TIME;

	        /// CONTROL ///

	        // Horizontal Movement
	        float target_speed = 0;
	        if (Input.IsActionPressed("move_left"))
		        target_speed += -1;
	        if (Input.IsActionPressed("move_right"))
		        target_speed +=  1;

	        target_speed *= WALK_SPEED;
	        player.linear_vel.x = Mathf.Lerp(player.linear_vel.x, target_speed, 0.1f);

	        // Jumping
	        if (player.on_floor && Input.IsActionJustPressed("jump"))
            {
                player.linear_vel.y = -JUMP_SPEED;
		        godotNode.node.GetNode<AudioStreamPlayer2D>("sound_jump").Play();
            }

            // We'll need this for some stuff
            var sprite = godotNode.node.GetNode<Sprite>("sprite");

            // Shooting
	        if (Input.IsActionJustPressed("shoot"))
            {
                var bullet = Spawn.FromDef(ActorDefs.Bullet, godotNode.node.GetParent(), godotNode.node.GetNode<Node2D>("sprite/bullet_shoot").GlobalPosition);
                (bullet.Component<Comp.GodotNode>().node as RigidBody2D).LinearVelocity = new Vector2(sprite.Scale.x * BULLET_VELOCITY, 0);
                (bullet.Component<Comp.GodotNode>().node as RigidBody2D).AddCollisionExceptionWith(godotNode.node);
		        godotNode.node.GetNode<AudioStreamPlayer2D>("sound_shoot").Play();
		        player.shoot_time = 0;
            }
		    

	        /// ANIMATION ///

	        string new_anim = "idle";

	        if (player.on_floor)
            {
                if (player.linear_vel.x < -SIDING_CHANGE_SPEED)
                {
                    sprite.Scale = new Vector2(-1, 1);
                    new_anim = "run";
                }

                if (player.linear_vel.x > SIDING_CHANGE_SPEED)
                {
                    sprite.Scale = new Vector2(1, 1);
                    new_anim = "run";
                }
            }
            else
            {
                // We want the character to immediately change facing side when the player
		        // tries to change direction, during air control.
		        // This allows for example the player to shoot quickly left then right.
		        if (Input.IsActionPressed("move_left") && !Input.IsActionPressed("move_right"))
			        sprite.Scale = new Vector2(-1, 1);
		        if (Input.IsActionPressed("move_right") && !Input.IsActionPressed("move_left"))
			        sprite.Scale = new Vector2(1, 1);

		        if (player.linear_vel.y < 0)
			        new_anim = "jumping";
		        else
			        new_anim = "falling";
            }


	        if (player.shoot_time < SHOOT_TIME_SHOW_WEAPON)
		        new_anim += "_weapon";

	        if (new_anim != player.anim)
            {
                player.anim = new_anim;
                godotNode.node.GetNode<AnimationPlayer>("anim").Play(player.anim);
            }
        }
    }

    static class MonsterMovement
    {
        public static void Execute()
        {

        }
    }

    static class BulletBehavior
    {
        public static void Execute()
        {

        }
    }

    static class MonsterDeath
    {
        public static void Execute()
        {

        }
    }

    static class CoinPickup
    {
        public static void Execute()
        {

        }
    }
}
