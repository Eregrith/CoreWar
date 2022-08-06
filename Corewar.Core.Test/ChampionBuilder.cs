using Corewar.Core.Enums;
using Corewar.Core.Memory;
using System;
using System.Collections.Generic;

namespace Corewar.Core.Test
{
    internal class ChampionBuilder
    {
        private List<MemoryCell> _instructions = new List<MemoryCell>();

        public ChampionBuilder AddSimpleDAT(int value)
        {
            _instructions.Add(new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, value));
            return this;
        }

        public SubInstructionBuilder AddOpcode(Opcodes op)
        {
            return new SubInstructionBuilder(op, this, _instructions);
        }

        public Champion Build()
        {
            return new Champion(_instructions.ToArray());
        }

        internal class SubInstructionBuilder
        {
            private readonly Opcodes _opcode;
            private Modifier _modifier;
            private AddressMode _mode_A = 0;
            private AddressMode _mode_B = 0;
            private int A = 0;
            private int B = 0;
            private readonly ChampionBuilder _parentBuilder;
            private readonly List<MemoryCell> _instructions;

            public SubInstructionBuilder(Opcodes op, ChampionBuilder parentBuilder, List<MemoryCell> instructions)
            {
                _opcode = op;
                _parentBuilder = parentBuilder;
                _instructions = instructions;
            }

            /// <summary>
            /// .mod
            /// </summary>
            public SubInstructionBuilder With_Modifier(Modifier mod)
            {
                _modifier = mod;
                return this;
            }

            /// <summary>
            /// #A
            /// </summary>
            public SubInstructionBuilder With_Operand_A_immediate(int a)
            {
                A = a;
                _mode_A = AddressMode.Immediate;
                return this;
            }

            /// <summary>
            /// $A
            /// </summary>
            public SubInstructionBuilder With_Operand_A_direct(int a)
            {
                A = a;
                _mode_A = AddressMode.Direct;
                return this;
            }

            /// <summary>
            /// @A
            /// </summary>
            public SubInstructionBuilder With_Operand_A_indirect(int a)
            {
                A = a;
                _mode_A = AddressMode.Indirect;
                return this;
            }

            /// <summary>
            /// <A
            /// </summary>
            public SubInstructionBuilder With_Operand_A_decrement(int a)
            {
                A = a;
                _mode_A = AddressMode.Decrement;
                return this;
            }

            /// <summary>
            /// >A
            /// </summary>
            public SubInstructionBuilder With_Operand_A_increment(int a)
            {
                A = a;
                _mode_A = AddressMode.Increment;
                return this;
            }

            /// <summary>
            /// #B
            /// </summary>
            public SubInstructionBuilder With_Operand_B_immediate(int b)
            {
                B = b;
                _mode_B = AddressMode.Immediate;
                return this;
            }

            /// <summary>
            /// $B
            /// </summary>
            public SubInstructionBuilder With_Operand_B_direct(int b)
            {
                B = b;
                _mode_B = AddressMode.Direct;
                return this;
            }

            /// <summary>
            /// @B
            /// </summary>
            public SubInstructionBuilder With_Operand_B_indirect(int b)
            {
                B = b;
                _mode_B = AddressMode.Indirect;
                return this;
            }

            /// <summary>
            /// <B
            /// </summary>
            public SubInstructionBuilder With_Operand_B_drecrement(int b)
            {
                B = b;
                _mode_B = AddressMode.Decrement;
                return this;
            }

            /// <summary>
            /// >B
            /// </summary>
            public SubInstructionBuilder With_Operand_B_increment(int b)
            {
                B = b;
                _mode_B = AddressMode.Increment;
                return this;
            }

            public ChampionBuilder End()
            {
                _instructions.Add(new MemoryCell(
                      _opcode
                    , _modifier
                    , _mode_A
                    , A
                    , _mode_B
                    , B
                ));
                return _parentBuilder;
            }
        }
    }
}
