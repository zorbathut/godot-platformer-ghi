using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Def.StaticReferences]
public static class ProcessDefs
{
    static ProcessDefs() { Def.StaticReferences.Initialized(); }

    public static Ghi.ProcessDef Tick;
}

[Def.StaticReferences]
public static class ActorDefs
{
    static ActorDefs() { Def.StaticReferences.Initialized(); }

    public static ActorDef Bullet;
    public static ActorDef Monster;
}