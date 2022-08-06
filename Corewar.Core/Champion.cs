using Corewar.Core.Memory;
using Corewar.Core.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core
{
    public class Champion
    {
        public bool IsAlive { get; set; } = true;
        public MemoryCell[] Instructions { get; }
        public string? Name => Identity?.Name;
        public string? Author => Identity?.Author;
        public string? Version => Identity?.Version;
        public string? Date => Identity?.Date;
        public string? Strategy => Identity?.Strategy;
        public int Origin => Identity?.Origin ?? 0;
        public ChampionIdentity? Identity { get; private set; } = null;

        public Champion(params MemoryCell[] instructions)
            : this(null, instructions)
        { }

        public Champion(ChampionIdentity? identity, MemoryCell[] instructions)
        {
            Instructions = instructions;
            Identity = identity;
        }
    }
}
