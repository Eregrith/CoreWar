namespace Corewar.Core.Parser
{
    internal class OperandValue
    {
        public int? Value { get; set; }
        public bool IsLabel => !String.IsNullOrEmpty(Label);
        public string? Label { get; set; }

        public OperandValue(int v)
        {
            Value = v;
        }

        public OperandValue(string label)
        {
            Label = label;
        }
    }
}
