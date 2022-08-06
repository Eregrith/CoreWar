using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Random
{
    public interface IRandomGenerator
    {
        int Next(int min, int max);
    }
}
