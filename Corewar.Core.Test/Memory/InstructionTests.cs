using Corewar.Core.Enums;
using Corewar.Core.Memory;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Test.Memory
{
    internal class InstructionTests
    {
        [Test]
        public void Should_Fail_On_DAT()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, 0)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            instruction.Success.Should().BeFalse();
        }

        [Test]
        public void Should_Succeed_On_legal_operation()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Direct, 0, AddressMode.Direct, 1),
                new MemoryCell()
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            instruction.Success.Should().BeTrue();
            instruction.NextInstructionPointer.Should().Be(1);
        }

        [Test]
        public void Should_Wrap_Memory_When_Reading()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Indirect, -1, AddressMode.Indirect, 3),
                MemoryCell.DAT(5),
                MemoryCell.DAT(-1),
                MemoryCell.DAT(1),
                MemoryCell.DAT(0),
                MemoryCell.DAT(2)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[4].Should().Be(memory[1]);
        }

        [Test]
        public void Should_Wrap_Memory_When_Writing()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Immediate, 0, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
                MemoryCell.DAT(0)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].Should().Be(memory[0]);
        }

        [Test]
        public void Should_Execute_MOV_I_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Direct, 0, AddressMode.Direct, 1),
                new MemoryCell()
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].Should().Be(memory[0]);
            memory[1].ANumber = 5;
            memory[0].ANumber.Should().NotBe(5);
        }

        [Test]
        public void Should_Execute_MOV_I_Properly_When_Both_Operands_Are_In_Indirect_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Indirect, 2, AddressMode.Indirect, 3),
                MemoryCell.DAT(5),
                MemoryCell.DAT(-1),
                MemoryCell.DAT(1),
                MemoryCell.DAT(0)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[4].Should().Be(memory[1]);
        }

        [Test]
        public void Should_Execute_MOV_I_Properly_When_Both_Operands_Are_In_Decrement_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Decrement, 2, AddressMode.Decrement, 3),
                MemoryCell.DAT(5),
                MemoryCell.DAT(0),
                MemoryCell.DAT(2),
                MemoryCell.DAT(0)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[4].Should().Be(memory[1]);
            memory[2].BNumber.Should().Be(-1);
            memory[3].BNumber.Should().Be(1);
        }

        [Test]
        public void Should_Execute_MOV_I_Properly_When_Both_Operands_Are_In_Increment_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.I, AddressMode.Increment, 2, AddressMode.Increment, 3),
                MemoryCell.DAT(5),
                MemoryCell.DAT(-1),
                MemoryCell.DAT(1),
                MemoryCell.DAT(0)
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[4].Should().Be(memory[1]);
            memory[2].BNumber.Should().Be(0);
            memory[3].BNumber.Should().Be(2);
        }

        [Test]
        public void Should_Execute_MOV_A_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 6, AddressMode.Immediate, 0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].ANumber.Should().Be(memory[1].ANumber);
        }

        [Test]
        public void Should_Execute_MOV_B_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.B, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, 6),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].BNumber.Should().Be(memory[1].BNumber);
        }

        [Test]
        public void Should_Execute_MOV_AB_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.AB, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].BNumber.Should().Be(memory[1].ANumber);
        }

        [Test]
        public void Should_Execute_MOV_BA_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.BA, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 23),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].ANumber.Should().Be(memory[1].BNumber);
        }

        [Test]
        public void Should_Execute_MOV_F_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.F, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 23),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].ANumber.Should().Be(memory[1].ANumber);
            memory[2].BNumber.Should().Be(memory[1].BNumber);
        }

        [Test]
        public void Should_Execute_MOV_X_Properly_When_Both_Operands_Are_In_Direct_Mode()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.X, AddressMode.Direct, 1, AddressMode.Direct, 2),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 23),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[2].ANumber.Should().Be(memory[1].BNumber);
            memory[2].BNumber.Should().Be(memory[1].ANumber);
        }

        [TestCase(Opcodes.MOV)]
        [TestCase(Opcodes.ADD)]
        [TestCase(Opcodes.SUB)]
        [TestCase(Opcodes.MUL)]
        [TestCase(Opcodes.DIV)]
        [TestCase(Opcodes.MOD)]
        [TestCase(Opcodes.DJN)]
        public void Should_Mark_Target_Cell_As_Written_By_Loaded_Champion_When_Executing_Writing_Opcodes(Opcodes opcode)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(opcode, Modifier.I, AddressMode.Direct, 0, AddressMode.Direct, 1),
                new MemoryCell()
            };
            int currentChampionIndex = 2;
            var instruction = new Instruction(memory, 0, currentChampionIndex);

            instruction.Execute();

            memory[1].IndexOfLastChampToWriteHere.Should().Be(currentChampionIndex);
        }

        [Test]
        public void Should_Execute_ADD_A_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, Modifier.A, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(8);
        }

        [Test]
        public void Should_Execute_ADD_B_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, Modifier.B, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].BNumber.Should().Be(20);
        }

        [Test]
        public void Should_Execute_ADD_AB_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, Modifier.AB, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].BNumber.Should().Be(22);
        }

        [Test]
        public void Should_Execute_ADD_BA_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, Modifier.BA, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(6);
        }

        [TestCase(Modifier.F)]
        [TestCase(Modifier.I)]
        public void Should_Execute_ADD_F_And_I_Properly(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, modifier, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(8);
            memory[1].BNumber.Should().Be(20);
        }

        [Test]
        public void Should_Execute_ADD_X_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.ADD, Modifier.X, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(22);
            memory[1].BNumber.Should().Be(6);
        }

        [Test]
        public void Should_Execute_SUB_A_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.SUB, Modifier.A, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(2);
        }

        [Test]
        public void Should_Execute_MUL_A_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MUL, Modifier.A, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 5, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(15);
        }

        [TestCase(Modifier.B)]
        [TestCase(Modifier.BA)]
        [TestCase(Modifier.F)]
        [TestCase(Modifier.X)]
        public void Should_fail_On_DIV_B_When_B_is_0(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, modifier, AddressMode.Immediate, 10, AddressMode.Direct, 0),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].BNumber.Should().Be(19);
            instruction.Success.Should().BeFalse();
        }

        [TestCase(Modifier.A)]
        [TestCase(Modifier.AB)]
        [TestCase(Modifier.F)]
        [TestCase(Modifier.X)]
        public void Should_fail_On_DIV_A_When_A_is_0(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, modifier, AddressMode.Immediate, 0, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(15);
            instruction.Success.Should().BeFalse();
        }

        [Test]
        public void Should_Execute_DIV_A_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, Modifier.A, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 19),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(5);
        }

        [Test]
        public void Should_Execute_DIV_B_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, Modifier.B, AddressMode.Immediate, 23, AddressMode.Direct, 3),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].BNumber.Should().Be(5);
        }

        [Test]
        public void Should_Execute_DIV_AB_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, Modifier.AB, AddressMode.Immediate, 3, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 25, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].BNumber.Should().Be(5);
        }

        [Test]
        public void Should_Execute_DIV_BA_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, Modifier.BA, AddressMode.Immediate, 53, AddressMode.Direct, 3),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(5);
        }

        [TestCase(Modifier.F)]
        [TestCase(Modifier.I)]
        public void Should_Execute_DIV_F_or_I_Properly(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, modifier, AddressMode.Immediate, 5, AddressMode.Direct, 3),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(3);
            memory[1].BNumber.Should().Be(5);
        }

        [Test]
        public void Should_Execute_DIV_X_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.DIV, Modifier.X, AddressMode.Immediate, 5, AddressMode.Direct, 3),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(5);
            memory[1].BNumber.Should().Be(3);
        }

        [Test]
        public void Should_Execute_MOD_A_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOD, Modifier.A, AddressMode.Immediate, 5, AddressMode.Direct, 3),
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 15, AddressMode.Immediate, 15),
            };
            var instruction = new Instruction(memory, 0);

            instruction.Execute();

            memory[1].ANumber.Should().Be(0);
        }

        [Test]
        public void Should_Execute_JMP_Properly()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.JMP, Modifier.A, AddressMode.Direct, -2, AddressMode.Immediate, 0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(0);
        }

        [TestCase(Modifier.A)]
        [TestCase(Modifier.BA)]
        public void Should_Execute_JMZ_A_And_BA_And_Jump_When_ANumber_Of_Target_Is_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 0, AddressMode.Direct, 123),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.A)]
        [TestCase(Modifier.BA)]
        public void Should_Execute_JMZ_A_And_BA_And_Not_Jump_When_ANumber_Of_Target_Is_Not_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 123),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [TestCase(Modifier.B)]
        [TestCase(Modifier.AB)]
        public void Should_Execute_JMZ_B_And_AB_And_Jump_When_BNumber_Of_Target_Is_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 123, AddressMode.Direct, 0),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.B)]
        [TestCase(Modifier.AB)]
        public void Should_Execute_JMZ_B_And_AB_And_Not_Jump_When_BNumber_Of_Target_Is_Not_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 123, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [TestCase(Modifier.F)]
        [TestCase(Modifier.X)]
        [TestCase(Modifier.I)]
        public void Should_Execute_JMZ_F_X_I_And_Jump_When_ANumber_And_BNumber_Of_Target_Is_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 0, AddressMode.Direct, 0),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.F, 0, 1)]
        [TestCase(Modifier.X, 1, 1)]
        [TestCase(Modifier.I, 1, 0)]
        public void Should_Execute_JMZ_F_X_I_And_Not_Jump_When_ANumber_Or_BNumber_Of_Target_Is_Not_Zero(Modifier modifier, int a, int b)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, a, AddressMode.Direct, b),
                new MemoryCell(Opcodes.JMZ, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_JMN_Like_JMZ_But_With_Non_Zero_Check()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 0),
                new MemoryCell(Opcodes.JMN, Modifier.A, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.A)]
        [TestCase(Modifier.BA)]
        public void Should_Execute_DJN_A_And_BA_And_Decrement_Target_ANumber_Then_Jump_If_Not_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 0),
                new MemoryCell(Opcodes.DJN, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            memory[1].ANumber.Should().Be(0);
            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.B)]
        [TestCase(Modifier.AB)]
        public void Should_Execute_DJN_B_And_AB_And_Decrement_Target_BNumber_Then_Jump_If_Not_Zero(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 0, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.DJN, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            memory[1].BNumber.Should().Be(0);
            instruction.NextInstructionPointer.Should().Be(4);
        }

        [TestCase(Modifier.F, 0, 1)]
        [TestCase(Modifier.X, 1, 1)]
        [TestCase(Modifier.I, 1, 0)]
        public void Should_Execute_DJN_F_X_I_And_Decrement_Target_ANumber_And_BNumber_Then_Jump_If_Any_Is_Non_Zero(Modifier modifier, int a, int b)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, a, AddressMode.Direct, b),
                new MemoryCell(Opcodes.DJN, modifier, AddressMode.Direct, 2, AddressMode.Direct, -1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 2);

            instruction.Execute();

            memory[1].ANumber.Should().Be(a - 1);
            memory[1].BNumber.Should().Be(b - 1);
            instruction.NextInstructionPointer.Should().Be(4);
        }

        [Test]
        public void Should_Execute_CMP_A_And_Skip_Next_Instruction_If_ANumber_Equals_ANumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.A, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_A_And_Not_Skip_Next_Instruction_If_ANumber_Not_Equals_ANumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.A, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 2, AddressMode.Direct, 0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(2);
        }

        [Test]
        public void Should_Execute_CMP_B_And_Skip_Next_Instruction_If_BNumber_Equals_BNumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.B, AddressMode.Immediate, 142, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 52, AddressMode.Direct, 1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_AB_And_Skip_Next_Instruction_If_ANumber_Equals_BNumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.AB, AddressMode.Immediate, 42, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_BA_And_Skip_Next_Instruction_If_BNumber_Equals_ANumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.BA, AddressMode.Immediate, 675, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_F_And_Skip_Next_Instruction_If_ABNumber_Equals_ABNumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.F, AddressMode.Immediate, 12, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 12, AddressMode.Direct, 1),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_X_And_Skip_Next_Instruction_If_ABNumber_Equals_BANumber_Of_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.CMP, Modifier.X, AddressMode.Immediate, 12, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 12),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_I_And_Skip_Next_Instruction_If_Whole_Instructions_Are_Equal()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 12),
                new MemoryCell(Opcodes.CMP, Modifier.I, AddressMode.Direct, -1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 12),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_CMP_I_And_Not_Skip_Next_Instruction_If_Whole_Instructions_Are_Not_Equal()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 2, AddressMode.Direct, 12),
                new MemoryCell(Opcodes.CMP, Modifier.I, AddressMode.Direct, -1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 1, AddressMode.Direct, 12),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(2);
        }

        [Test]
        public void Should_Execute_SLT_A_And_Skip_Next_Instruction_If_ANumber_Is_Less_Than_Target_ANumber()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, Modifier.A, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_SLT_B_And_Skip_Next_Instruction_If_BNumber_Is_Less_Than_Target_BNumber()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, Modifier.B, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_SLT_AB_And_Skip_Next_Instruction_If_ANumber_Is_Less_Than_Target_BNumber()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, Modifier.AB, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_SLT_BA_And_Skip_Next_Instruction_If_BNumber_Is_Less_Than_Target_ANumber()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, Modifier.BA, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [TestCase(Modifier.F)]
        [TestCase(Modifier.I)]
        public void Should_Execute_SLT_F_And_I_And_Skip_Next_Instruction_If_ABNumber_Is_Less_Than_Target_ABNumber(Modifier modifier)
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, modifier, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_SLT_X_And_Skip_Next_Instruction_If_ABNumber_Is_Less_Than_Target_BANumber()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SLT, Modifier.X, AddressMode.Immediate, 1, AddressMode.Direct, 1),
                new MemoryCell(Opcodes.MOV, Modifier.A, AddressMode.Direct, 4, AddressMode.Direct, 42),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(3);
        }

        [Test]
        public void Should_Execute_SPL_To_Declare_New_Task_Starting_At_Target()
        {
            MemoryCell[] memory = new MemoryCell[]
            {
                MemoryCell.DAT(0),
                new MemoryCell(Opcodes.SPL, Modifier.A, AddressMode.Direct, 3, AddressMode.Direct, 0),
                MemoryCell.DAT(0),
                MemoryCell.DAT(0),
                MemoryCell.DAT(0),
                MemoryCell.DAT(0),
                MemoryCell.DAT(0),
            };
            var instruction = new Instruction(memory, 1);

            instruction.Execute();

            instruction.NextInstructionPointer.Should().Be(2);
            instruction.NewTaskSpawningAt.Should().Be(4);
        }
    }
}
