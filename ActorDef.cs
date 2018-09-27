using Godot;
using System.Collections.Generic;

public class ActorDef : Def.Def
{
    public Ghi.EntityDef entityDef;
    public string scene;
    
    // derived post-load below this line

    public string FullScenePath
    {
        get
        {
            return "res://" + scene + ".tscn";
        }
    }

    public Node SceneInstance
    {
        get
        {
            return resource.Instance();
        }
    }

    private PackedScene resource;
    private readonly static Dictionary<string, ActorDef> Lookup = new Dictionary<string, ActorDef>();

    public override IEnumerable<string> PostLoad()
    {
        resource = ResourceLoader.Load(FullScenePath) as PackedScene;
        
        if (resource == null)
        {
            yield return $"{FullScenePath} doesn't exist";
        }

        if (Lookup.ContainsKey(FullScenePath))
        {
            yield return $"{FullScenePath} is also used by {Lookup[FullScenePath]}";
        }
        Lookup[FullScenePath] = this;
    }

    public static ActorDef FindActorFromPath(string path)
    {
        return Lookup.TryGetValue(path);
    }
}
