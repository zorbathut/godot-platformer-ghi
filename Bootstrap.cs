using Godot;
using System.Linq;

class Bootstrap : Node
{
    bool processed = false;

    public override void _Ready()
    {
        base._Ready();

        Def.Config.InfoHandler = str => GD.Print("INF: " + str);
        Def.Config.WarningHandler = str => GD.Print("WRN: " + str);
        Def.Config.ErrorHandler = str => GD.Print("ERR: " + str);

        
        // Spool up defs system
        var parser = new Def.Parser();
        foreach (var fname in Util.GetFilesFromDir("res://defs"))
        {
            parser.AddString(Util.GetFileAsString(fname));
        }
        parser.Finish();

        Ghi.Environment.Startup();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (!processed)
        {
            foreach (var node in GetTree().GetNodes().OfType<Node2D>())
            {
                if (ActorDef.FindActorFromPath(node.Filename) is var actorDef && actorDef != null)
                {
                    Spawn.FromNode(node, actorDef);
                }
            }

            processed = true;
        }

        Dbg.Inf($"proci {Ghi.Environment.Count}");
        Ghi.Environment.Process(ProcessDefs.Tick);
    }
}
