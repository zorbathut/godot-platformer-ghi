using Godot;
using System.Collections.Generic;
using System.Linq;

static class Spawn
{
    private static Dictionary<Node, Ghi.Entity> LookupTable = new Dictionary<Node, Ghi.Entity>();

    public static Ghi.Entity FromNode(Node2D node, ActorDef def)
    {
        var newEntity = new Ghi.Entity(def.entityDef, node);

        newEntity.Component<Comp.ActorDef>().def = def;
        
        LookupTable[newEntity.Component<Node>()] = newEntity;
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

    public static void Detach(Ghi.Entity entity)
    {
        LookupTable.Remove(entity.Component<Node>());
        Ghi.Environment.Remove(entity);
    }

    public static Ghi.Entity Lookup(Node node)
    {
        return LookupTable.TryGetValue(node);
    }
}
