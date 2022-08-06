using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Exceptions
{
    internal class CantPositionChampionRandomlyException : Exception
    {
        public CantPositionChampionRandomlyException(Champion c)
        {
            Champion = c;
        }

        public Champion Champion { get; }
    }
}
