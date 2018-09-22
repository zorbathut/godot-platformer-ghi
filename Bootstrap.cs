using Godot;

class Bootstrap : Node
{
    public override void _Ready()
    {
        base._Ready();

        // Spool up defs system
        var parser = new Def.Parser();
        foreach (var fname in Util.GetFilesFromDir("res://defs"))
        {
            parser.AddString(Util.GetFileAsString(fname));
        }
        parser.Finish();

        Ghi.Environment.Startup();
    }
}
