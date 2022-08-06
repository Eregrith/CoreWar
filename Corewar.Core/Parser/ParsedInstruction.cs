using Corewar.Core.Enums;

namespace Corewar.Core.Parser
{
    internal struct ParsedInstruction
    {
        public string? Label;
        public Opcodes Opcode;
        public Modifier Modifier;
        public AddressMode AMode;
        public OperandValue ANumber;
        public AddressMode BMode;
        public OperandValue BNumber;

        public ParsedInstruction(
            string? label,
            Opcodes opcode,
            Modifier modifier,
            AddressMode aMode,
            OperandValue aNumber,
            AddressMode bMode,
            OperandValue bNumber)
        {
            Label = label;
            Opcode = opcode;
            Modifier = modifier;
            AMode = aMode;
            ANumber = aNumber;
            BMode = bMode;
            BNumber = bNumber;
        }
    }
}
