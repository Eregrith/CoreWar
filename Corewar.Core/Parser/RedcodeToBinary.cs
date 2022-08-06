using Corewar.Core.Enums;
using Corewar.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Parser
{
    internal class RedcodeToBinary : IRedcodeToBinary
    {
        public Champion Parse(string input)
        {
            var parsedInstructions = new List<ParsedInstruction>();
            var lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            ChampionIdentity championIdentity = new ChampionIdentity();
            OperandValue origin = ParseLines(parsedInstructions, lines, championIdentity);
            SetChampionOrigin(parsedInstructions, championIdentity, origin);
            List<MemoryCell> instructions = GetInstructionsFromParsed(parsedInstructions);
            return new Champion(championIdentity, instructions.ToArray());
        }

        private void SetChampionOrigin(List<ParsedInstruction> parsedInstructions, ChampionIdentity championIdentity, OperandValue origin)
        {
            if (origin.IsLabel)
            {
                championIdentity.Origin = FindLabelRelativeFrom(origin.Label!, parsedInstructions.First(p => p.Opcode != Opcodes.EQU), parsedInstructions);
            }
            else
            {
                championIdentity.Origin = origin.Value!.Value;
            }
        }

        private static OperandValue ParseLines(List<ParsedInstruction> parsedInstructions, string[] lines, ChampionIdentity championIdentity)
        {
            OperandValue origin = new OperandValue(0);
            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.StartsWith("END"))
                {
                    break;
                }
                origin = ParseTrimmedLine(parsedInstructions, championIdentity, origin, line, trimmedLine);
            }

            return origin;
        }

        private static OperandValue ParseTrimmedLine(List<ParsedInstruction> parsedInstructions, ChampionIdentity championIdentity, OperandValue origin, string line, string trimmedLine)
        {
            if (trimmedLine.StartsWith("ORG"))
            {
                origin = GetOrigin(line);
            }
            else if (!trimmedLine.StartsWith(';'))
            {
                ExtractInstruction(trimmedLine, parsedInstructions);
            }
            else
            {
                FillChampionIdentity(championIdentity, trimmedLine);
            }

            return origin;
        }

        private static OperandValue GetOrigin(string line)
        {
            OperandValue origin;
            string originOperand = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1];
            if (IsANumber(originOperand))
            {
                origin = new OperandValue(int.Parse(originOperand));
            }
            else
            {
                origin = new OperandValue(originOperand);
            }

            return origin;
        }

        private static void FillChampionIdentity(ChampionIdentity championIdentity, string line)
        {
            if (line.StartsWith(";name"))
            {
                int indexOfSpace = line.IndexOf(' ');
                championIdentity.Name = line[indexOfSpace..].Trim();
            }
            else if (line.StartsWith(";author"))
            {
                int indexOfSpace = line.IndexOf(' ');
                championIdentity.Author = line[indexOfSpace..].Trim();
            }
            else if (line.StartsWith(";version"))
            {
                int indexOfSpace = line.IndexOf(' ');
                championIdentity.Version = line[indexOfSpace..].Trim();
            }
            else if (line.StartsWith(";date"))
            {
                int indexOfSpace = line.IndexOf(' ');
                championIdentity.Date = line[indexOfSpace..].Trim();
            }
            else if (line.StartsWith(";strategy"))
            {
                int indexOfSpace = line.IndexOf(' ');
                championIdentity.Strategy = line[indexOfSpace..].Trim();
            }
        }

        private List<MemoryCell> GetInstructionsFromParsed(List<ParsedInstruction> parsedInstructions)
        {
            List<MemoryCell> instructions = new List<MemoryCell>();

            foreach (var instruction in parsedInstructions)
            {
                if (instruction.Opcode != Opcodes.EQU)
                {
                    int aValue = instruction.ANumber.IsLabel ? FindLabelRelativeFrom(instruction.ANumber.Label!, instruction, parsedInstructions) : instruction.ANumber.Value!.Value;
                    int bValue = instruction.BNumber.IsLabel ? FindLabelRelativeFrom(instruction.BNumber.Label!, instruction, parsedInstructions) : instruction.BNumber.Value!.Value;

                    instructions.Add(new MemoryCell(
                        instruction.Opcode,
                        instruction.Modifier,
                        instruction.AMode,
                        aValue,
                        instruction.BMode,
                        bValue
                    ));
                }
            }
            return instructions;
        }

        private int FindLabelRelativeFrom(string labelToFind, ParsedInstruction instruction, List<ParsedInstruction> parsedInstructions)
        {
            if (parsedInstructions.Any(p => p.Label == labelToFind && p.Opcode == Opcodes.EQU))
            {
                return parsedInstructions.First(p => p.Label == labelToFind && p.Opcode == Opcodes.EQU).ANumber.Value!.Value;
            }

            var EQUlessInstructions = parsedInstructions.Where(p => p.Opcode != Opcodes.EQU).ToList();
            int indexOfMe = EQUlessInstructions.IndexOf(instruction);
            int indexOfTarget = EQUlessInstructions.IndexOf(parsedInstructions.FirstOrDefault(p => p.Label == labelToFind));

            return indexOfTarget - indexOfMe;
        }

        private static void ExtractInstruction(string input, List<ParsedInstruction> instructions)
        {
            string[] parts = input.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string? label = null;
            int labelOffset = 0;
            Opcodes opcode = GetOpcodeFor(parts[0]);
            if (opcode == Opcodes.Unknown)
            {
                label = parts[0];
                opcode = GetOpcodeFor(parts[1]);
                labelOffset = 1;
            }
            Modifier modifier = GetModifierFor(parts[labelOffset]);
            AddressMode bMode = AddressMode.Immediate;
            OperandValue bValue = new OperandValue(0);
            (AddressMode aMode, OperandValue aValue) = GetModeAndValue(parts[1 + labelOffset]);
            if (OpcodeHasBValue(opcode))
            {
                (bMode, bValue) = GetModeAndValue(parts[2 + labelOffset]);
            }
            instructions.Add(new ParsedInstruction(
                label,
                opcode,
                modifier,
                aMode,
                aValue,
                bMode,
                bValue
            ));
        }

        private static (AddressMode, OperandValue) GetModeAndValue(string operand)
        {
            AddressMode mode;
            int startIndex = 1;
            if (operand[0] == '#')
                mode = AddressMode.Immediate;
            else if (operand[0] == '@')
                mode = AddressMode.Indirect;
            else if (operand[0] == '<')
                mode = AddressMode.Decrement;
            else if (operand[0] == '>')
                mode = AddressMode.Increment;
            else
            {
                startIndex = operand[0] != '$' ? 0 : 1;
                mode = AddressMode.Direct;
            }
            OperandValue value;
            string modelessValue = operand.Substring(startIndex);
            if (IsANumber(modelessValue))
            {
                value = new OperandValue(int.Parse(modelessValue));
            }
            else
            {
                value = new OperandValue(modelessValue);
            }
            return (mode, value);
        }

        private static bool IsANumber(string modelessValue)
        {
            return (modelessValue[0] <= '9' && modelessValue[0] >= '0')
                 || modelessValue[0] == '-'
                 || modelessValue[0] == '+';
        }

        private static Opcodes GetOpcodeFor(string input)
        {
            if (input.StartsWith("DAT")) return Opcodes.DAT;
            if (input.StartsWith("MOV")) return Opcodes.MOV;
            if (input.StartsWith("ADD")) return Opcodes.ADD;
            if (input.StartsWith("SUB")) return Opcodes.SUB;
            if (input.StartsWith("MUL")) return Opcodes.MUL;
            if (input.StartsWith("DIV")) return Opcodes.DIV;
            if (input.StartsWith("MOD")) return Opcodes.MOD;
            if (input.StartsWith("JMP")) return Opcodes.JMP;
            if (input.StartsWith("JMZ")) return Opcodes.JMZ;
            if (input.StartsWith("JMN")) return Opcodes.JMN;
            if (input.StartsWith("DJN")) return Opcodes.DJN;
            if (input.StartsWith("CMP")) return Opcodes.CMP;
            if (input.StartsWith("SLT")) return Opcodes.SLT;
            if (input.StartsWith("SPL")) return Opcodes.SPL;
            if (input.StartsWith("EQU")) return Opcodes.EQU;
            return Opcodes.Unknown;
        }

        private static Modifier GetModifierFor(string input)
        {
            if (input.EndsWith(".A")) return Modifier.A;
            if (input.EndsWith(".B")) return Modifier.B;
            if (input.EndsWith(".AB")) return Modifier.AB;
            if (input.EndsWith(".BA")) return Modifier.BA;
            if (input.EndsWith(".X")) return Modifier.X;
            if (input.EndsWith(".F")) return Modifier.F;
            if (input.EndsWith(".I")) return Modifier.I;
            return Modifier.F;
        }

        private static bool OpcodeHasBValue(Opcodes opcode)
        {
            return opcode != Opcodes.JMP
                && opcode != Opcodes.SPL
                && opcode != Opcodes.EQU;
        }
    }
}
