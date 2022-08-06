using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Enums
{
    internal static class Constants
    {
        public const int VALUE_SIZE = 10;
        public const int MODE_SIZE = 3;
        public const int OPCODE_OFFSET = VALUE_SIZE * 2 + MODE_SIZE * 2;
        public const int A_MODE_OFFSET = VALUE_SIZE * 2 + MODE_SIZE;
        public const int B_MODE_OFFSET = VALUE_SIZE * 2;
        public const int A_VALUE_OFFSET = VALUE_SIZE;
    }
}
