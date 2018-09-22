using Godot;
using System.Collections.Generic;

class Util
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
}
