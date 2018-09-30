using Godot;
using System.Collections.Generic;
using System.Linq;

static class Util
{
    public static IEnumerable<string> GetFilesFromDir(string dirname)
    {
        var dir = new Directory();
        dir.Open(dirname);
        dir.ListDirBegin(skipNavigational: true);
        while (true)
        {
            string fname = dir.GetNext();
            if (fname == "")
            {
                break;
            }

            yield return dirname + "/" + fname;
        }
    }

    public static string GetFileAsString(string path)
    {
        var file = new File();
        file.Open(path, (int)File.ModeFlags.Read);
        var result = file.GetAsText();
        file.Close();
        return result;
    }

    public static IEnumerable<Node> GetNodes(this SceneTree sceneTree)
    {
        return sceneTree.GetRoot().GetAllChildren();
    }

    public static IEnumerable<Node> GetDirectChildren(this Node node)
    {
        return node.GetChildren().OfType<Node>();
    }

    public static IEnumerable<Node> GetAllChildren(this Node node)
    {
        yield return node;

        foreach (var child in node.GetDirectChildren())
        {
            foreach (var result in child.GetAllChildren())
            {
                yield return result;
            }
        }
    }

    public static V TryGetValue<T, V>(this Dictionary<T, V> dict, T key)
    {
        dict.TryGetValue(key, out V holder);
        return holder;
    }

    public static bool Collide(this CollisionShape2D lhs, CollisionShape2D rhs)
    {
        return lhs.GetShape().Collide(lhs.GlobalTransform, rhs.GetShape(), rhs.GlobalTransform);
    }
}
