using Corewar.Core.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Memory
{
    [DebuggerDisplay("{ToString()}")]
    public struct MemoryCell
    {
        public Opcodes Opcode { get; set; }
        public Modifier Modifier { get; set; }
        public AddressMode AMode { get; set; }
        public int ANumber { get; set; }
        public AddressMode BMode { get; set; }
        public int BNumber { get; set; }
        public int IndexOfLastChampToWriteHere { get; set; } = -1;

        public MemoryCell(Opcodes opcode, Modifier modifier, AddressMode aMode, int aNumber, AddressMode bMode, int bNumber)
        {
            Opcode = opcode;
            Modifier = modifier;
            AMode = aMode;
            ANumber = aNumber;
            BMode = bMode;
            BNumber = bNumber;
        }

        public static MemoryCell DAT(int value)
        {
            return new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, value);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MemoryCell other))
                return false;
            return other.Opcode == this.Opcode
                && other.Modifier == this.Modifier
                && other.AMode == this.AMode
                && other.ANumber == this.ANumber
                && other.BMode == this.BMode
                && other.BNumber == this.BNumber;
        }

        public override string ToString()
        {
            return $"{Opcode}.{Modifier}    {GetModeChar(AMode)}{ANumber},  {GetModeChar(BMode)}{BNumber}";
        }

        private char GetModeChar(AddressMode mode)
        {
            switch (mode)
            {
                case AddressMode.Immediate:
                    return '#';
                case AddressMode.Direct:
                    return '$';
                case AddressMode.Indirect:
                    return '@';
                case AddressMode.Decrement:
                    return '<';
                case AddressMode.Increment:
                    return '>';
                default:
                    return ' ';
            }
        }
    }
}
