using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Exceptions
{
    internal class ChampionExceedsInstructionLimitException : Exception
    {
        public Champion Champion { get; private set; }

        public ChampionExceedsInstructionLimitException(Champion c)
        {
            Champion = c;
        }
    }
}
