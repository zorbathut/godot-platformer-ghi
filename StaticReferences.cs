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
