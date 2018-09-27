using Godot;
using System.Collections.Generic;
using System.Linq;

static class Spawn
{
    public static Ghi.Entity FromNode(Node2D node, ActorDef def)
    {
        var newEntity = new Ghi.Entity(def.entityDef);

        newEntity.Component<Comp.ActorDef>().def = def;

        newEntity.Component<Comp.GodotNode>().node = node;
        newEntity.Component<Comp.GodotNode>().kinematicBody = node as KinematicBody2D;

        if (newEntity.Component<Comp.Position>() is var position && position != null)
        {
            position.position = node.GlobalPosition;
        }
        
        Ghi.Environment.Add(newEntity);
        return newEntity;
    }

    public static Ghi.Entity FromDef(ActorDef def, Node parent, Vector2 position)
    {
        var node = def.SceneInstance as Node2D;
        parent.AddChild(node);
        node.SetGlobalPosition(position);

        return FromNode(node, def);
    }
}
