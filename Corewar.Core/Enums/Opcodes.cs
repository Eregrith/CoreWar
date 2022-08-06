using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Enums
{
    public enum Opcodes
    {
        Unknown = -1,
        DAT,
        MOV,
        ADD,
        SUB,
        MUL,
        DIV,
        MOD,
        JMP,
        JMZ,
        JMN,
        DJN,
        CMP,
        SLT,
        SPL,
        EQU
    }
}
