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
        newEntity.Component<Comp.Position>().position = node.GlobalPosition;
        Ghi.Environment.Add(newEntity);
        return newEntity;
    }
}
