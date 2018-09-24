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
        public static void Execute()
        {

        }
    }

    static class PlayerMovement
    {
        public static void Execute(Comp.Player player, Comp.Position position, Comp.GodotNode godotNode)
        {
            player.phase += 0.01f;
            godotNode.node.SetGlobalPosition(position.position + new Vector2(Mathf.Cos(player.phase), Mathf.Sin(player.phase)) * 100);

            Dbg.Inf($"movin movin movin {player.phase} {godotNode.node.GetGlobalPosition()}");
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
