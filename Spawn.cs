using Godot;
using System.Collections.Generic;
using System.Linq;

static class Spawn
{
    public static Ghi.Entity FromNode(Node2D node, ActorDef def)
    {
        var newEntity = new Ghi.Entity(def.entityDef, node);

        newEntity.Component<Comp.ActorDef>().def = def;
        
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
