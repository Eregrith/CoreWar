namespace Corewar.Core
{
    public class NextMove
    {
        public Champion Champion { get; set; }
        public int InstructionPointer { get; set; }

        public NextMove(Champion c, int ip)
        {
            Champion = c;
            InstructionPointer = ip;
        }
    }
}
