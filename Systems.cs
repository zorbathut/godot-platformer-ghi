using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Systems
{
    static class PlatformMovement
    {
        public static void Execute(Comp.Godot.Platform node, Comp.Global global_ro)
        {
            node.accum += global_ro.delta * (1.0f / node.cycle) * Mathf.Pi * 2;
            node.accum %= Mathf.Pi * 2;
            var d = Mathf.Sin(node.accum);
            var xf = Transform2D.Identity;
            xf[2] = node.motion * d;

            node.GetNode<Node2D>("platform").Transform = xf;
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

        public static void Execute(Comp.Player player, KinematicBody2D node, Comp.Global global_ro)
        {
            // increment counters
	        player.onair_time += global_ro.delta;
	        player.shoot_time += global_ro.delta;        

            /// MOVEMENT ///

	        // Apply Gravity
	        player.linear_vel += global_ro.delta * GRAVITY_VEC;
	        // Move and Slide
	        player.linear_vel = node.MoveAndSlide(player.linear_vel, floorNormal: FLOOR_NORMAL);
	        // Detect Floor
	        if (node.IsOnFloor())
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
		        node.GetNode<AudioStreamPlayer2D>("sound_jump").Play();
            }

            // We'll need this for some stuff
            var sprite = node.GetNode<Sprite>("sprite");

            // Shooting
	        if (Input.IsActionJustPressed("shoot"))
            {
                var bullet = Spawn.FromDef(ActorDefs.Bullet, node.GetParent(), node.GetNode<Node2D>("sprite/bullet_shoot").GlobalPosition);
                bullet.Component<Comp.Projectile>().linear_velocity = new Vector2(sprite.Scale.x * BULLET_VELOCITY, 0);
		        node.GetNode<AudioStreamPlayer2D>("sound_shoot").Play();
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
                node.GetNode<AnimationPlayer>("anim").Play(player.anim);
            }
        }
    }

    static class MonsterMovement
    {
        static readonly Vector2 GRAVITY_VEC = new Vector2(0, 900);
        static readonly Vector2 FLOOR_NORMAL = new Vector2(0, -1);

        const float WALK_SPEED = 70;

        public static void Execute(Comp.Monster monster, Godot.KinematicBody2D body, Comp.Global global_ro)
        {
            var new_anim = "idle";

	        if (monster.state == Comp.Monster.State.Walking)
            {
                monster.linear_velocity += GRAVITY_VEC * global_ro.delta;
		        monster.linear_velocity.x = monster.direction * WALK_SPEED;
		        monster.linear_velocity = body.MoveAndSlide(monster.linear_velocity, FLOOR_NORMAL);

		        if (!body.GetNode<RayCast2D>("detect_floor_left").IsColliding() || body.GetNode<RayCast2D>("detect_wall_left").IsColliding())
			        monster.direction = 1;

		        if (!body.GetNode<RayCast2D>("detect_floor_right").IsColliding() || body.GetNode<RayCast2D>("detect_wall_right").IsColliding())
			        monster.direction = -1;

		        body.GetNode<Sprite>("sprite").Scale = new Vector2(monster.direction, 1);
		        new_anim = "walk";
            }
	        else
            {
                new_anim = "explode";
            }

	        if (monster.anim != new_anim)
            {
		        monster.anim = new_anim;
		        body.GetNode<AnimationPlayer>("anim").Play(monster.anim);
            }
        }
    }

    static class BulletBehavior
    {
        public static void Execute(Comp.Global global_ro)
        {
            var bullets = Ghi.Environment.List.Where(e => e.ComponentRO<Comp.ActorDef>().def == ActorDefs.Bullet).ToArray();

            List<Ghi.Entity> bulletsLive = new List<Ghi.Entity>();

            // Move all the bulletses
            foreach (var bulletEntity in bullets)
            {
                var bulletBody = bulletEntity.Component<KinematicBody2D>();

                var result = bulletBody.MoveAndCollide(bulletEntity.Component<Comp.Projectile>().linear_velocity * global_ro.delta);

                if (result != null)
                {
                    bulletBody.GetNode<AnimationPlayer>("anim").Play("shutdown");
                    Ghi.Environment.Remove(bulletEntity);
                }
            }
        }
    }

    static class CoinPickup
    {
        public static void Execute()
        {
            var players = Ghi.Environment.List.Where(e => e.ComponentRO<Comp.Player>() != null).ToArray();
            var coins = Ghi.Environment.List.Where(e => e.ComponentRO<Comp.Collectible>() != null).ToArray();
                
            foreach (var playerentity in players)
            {
                var playercollide = playerentity.ComponentRO<KinematicBody2D>().GetNode<CollisionShape2D>("collision");

                foreach (var coinentity in coins)
                {
                    var coincollide = coinentity.ComponentRO<Area2D>().GetNode<CollisionShape2D>("collision");
                    
                    if (playercollide.Collide(coincollide))
                    {
                        coinentity.ComponentRO<Area2D>().GetNode<AnimationPlayer>("anim").Play("taken");
                        Ghi.Environment.Remove(coinentity);
                    }
                }
            }
        }
    }
}
