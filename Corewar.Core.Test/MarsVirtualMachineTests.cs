using Corewar.Core.Enums;
using Corewar.Core.Exceptions;
using Corewar.Core.Memory;
using Corewar.Core.Parser;
using Corewar.Core.Random;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Test
{
    internal class MarsVirtualMachineTests
    {
        [Test]
        public void Should_Take_Given_Size_With_Empty_Memory()
        {
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, null);

            testedVm.Memory.Should().HaveCount(100);
            testedVm.Memory.Should().AllSatisfy(b => b.Should().Be(
                new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, 0)
            ));
        }

        [Test]
        public void Should_Load_One_Champion_In_Random_Space()
        {
            Champion c = new Champion(new MemoryCell());
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);

            testedVm.LoadChampion(c);

            testedVm.Memory[randomSpace].Should().Be(c.Instructions[0]);
        }

        [Test]
        public void Should_Set_Champion_First_Instruction_To_Its_Origin_Value()
        {
            Champion c = new Champion(new ChampionIdentity { Origin = 2 }, new[] { new MemoryCell(), new MemoryCell(), new MemoryCell() });
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);

            testedVm.LoadChampion(c);

            testedVm.Memory[randomSpace].Should().Be(c.Instructions[0]);
            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + c.Origin);
        }

        [Test]
        public void Should_Reject_Champion_When_It_Is_Larger_Than_InstructionLimit()
        {
            Champion c = new Champion(new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell());
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.InstructionLimit = 4;

            var loadAction = () => testedVm.LoadChampion(c);

            loadAction.Should().Throw<ChampionExceedsInstructionLimitException>().Which.Champion.Should().Be(c);
        }

        [Test]
        public void Should_Load_Champions_Separate_From_Previous_Champion_According_To_NonRandom_Separation_When_It_Is_Set()
        {
            Champion c1 = new Champion(new MemoryCell());
            Champion c2 = new Champion(new MemoryCell());
            Champion c3 = new Champion(new MemoryCell());
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 6;

            testedVm.LoadChampion(c1);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Memory[randomSpace].Should().Be(c1.Instructions[0]);
            testedVm.Memory[randomSpace + testedVm.Separation].Should().Be(c2.Instructions[0]);
            testedVm.Memory[randomSpace + testedVm.Separation + testedVm.Separation].Should().Be(c3.Instructions[0]);
        }

        [Test]
        public void Should_Load_Second_Champion_At_Random_Space()
        {
            Champion c1 = new Champion(new MemoryCell());
            Champion c2 = new Champion(new MemoryCell());
            int randomSpace = 24;
            int randomSpaceForSecondChamp = 52;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.SetupSequence(m => m.Next(0, 100))
                .Returns(randomSpace)
                .Returns(randomSpaceForSecondChamp);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.LoadChampion(c1);

            testedVm.LoadChampion(c2);

            testedVm.Memory[randomSpaceForSecondChamp].Should().Be(c2.Instructions[0]);
        }

        [Test]
        public void Should_Mark_Cells_As_Written_By_Loaded_Champion()
        {
            Champion c1 = new Champion(new MemoryCell(), new MemoryCell());
            Champion c2 = new Champion(new MemoryCell(), new MemoryCell());
            int randomSpace = 24;
            int randomSpaceForSecondChamp = 52;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.SetupSequence(m => m.Next(0, 100))
                .Returns(randomSpace)
                .Returns(randomSpaceForSecondChamp);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            
            testedVm.LoadChampion(c1);
            testedVm.LoadChampion(c2);

            testedVm.Memory[0].IndexOfLastChampToWriteHere.Should().Be(-1);
            testedVm.Memory[randomSpace].IndexOfLastChampToWriteHere.Should().Be(0);
            testedVm.Memory[randomSpace + 1].IndexOfLastChampToWriteHere.Should().Be(0);
            testedVm.Memory[randomSpaceForSecondChamp].IndexOfLastChampToWriteHere.Should().Be(1);
            testedVm.Memory[randomSpaceForSecondChamp + 1].IndexOfLastChampToWriteHere.Should().Be(1);
        }

        [Test]
        public void Should_Retry_If_Champion_Would_Load_Overlapping_Another_Big_One()
        {
            Champion c1 = new Champion(new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell());
            Champion c2 = new Champion(new MemoryCell());
            int randomSpace = 24;
            int randomSpaceForSecondChamp = 52;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.SetupSequence(m => m.Next(0, 100))
                .Returns(randomSpace)
                .Returns(randomSpace + 3)
                .Returns(randomSpace + 4)
                .Returns(randomSpaceForSecondChamp);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.LoadChampion(c1);

            testedVm.LoadChampion(c2);

            testedVm.Memory[randomSpaceForSecondChamp].Should().Be(c2.Instructions[0]);
        }

        [Test]
        public void Should_Retry_If_Champion_Would_Load_Overlapping_Minimum_Spacing_When_There_Is_Minimum_Spacing()
        {
            Champion c1 = new Champion(new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell());
            Champion c2 = new Champion(new MemoryCell());
            int randomSpace = 24;
            int randomSpaceForSecondChamp = randomSpace + 20;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.SetupSequence(m => m.Next(0, 100))
                .Returns(randomSpace)
                .Returns(randomSpace + 18)
                .Returns(randomSpace + 19)
                .Returns(randomSpaceForSecondChamp);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.MinimumSpacing = 20;
            testedVm.LoadChampion(c1);

            testedVm.LoadChampion(c2);

            testedVm.Memory[randomSpaceForSecondChamp].Should().Be(c2.Instructions[0]);
        }

        [Test]
        public void Should_Throw_If_Retries_Exceed_50()
        {
            Champion c1 = new Champion(new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell());
            Champion c2 = new Champion(new MemoryCell());
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            var sequence = randomMock.SetupSequence(m => m.Next(0, 100));
            for (int i = 0; i < 51; i++)
            {
                sequence.Returns(randomSpace);
            }
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.LoadChampion(c1);

            var testedAction = () => testedVm.LoadChampion(c2);

            testedAction.Should().Throw<CantPositionChampionRandomlyException>()
                .Which.Champion.Should().Be(c2);
        }

        [Test]
        public void Should_Wrap_Memory_When_Loading_Big_Champion()
        {
            Champion c = new Champion(new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell(), new MemoryCell());
            int randomSpace = 7;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 10)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(10, randomMock.Object);

            testedVm.LoadChampion(c);

            testedVm.Memory[randomSpace].Should().Be(c.Instructions[0]);
            testedVm.Memory[randomSpace + 1].Should().Be(c.Instructions[1]);
            testedVm.Memory[randomSpace + 2].Should().Be(c.Instructions[2]);
            testedVm.Memory[0].Should().Be(c.Instructions[3]);
            testedVm.Memory[1].Should().Be(c.Instructions[4]);
        }

        [Test]
        public void Should_Wrap_Memory_When_Separation_Is_Set()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 99;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;

            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.Step();

            testedVm.Memory[99].Should().Be(c.Instructions[0]);
            testedVm.Memory[9].Should().Be(c2.Instructions[0]);
            testedVm.NextMove.InstructionPointer.Should().Be(9);
        }

        [Test]
        public void Should_Execute_Current_Champion_Current_Instruction_When_Stepping()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();

            testedVm.Memory[randomSpace + 1].Should().Be(testedVm.Memory[randomSpace]);
            testedVm.Memory[randomSpace + 1].IndexOfLastChampToWriteHere.Should().Be(0);
        }

        [Test]
        public void Should_Alternate_Champions_After_Stepping()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c2);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + testedVm.Separation);
        }

        [Test]
        public void Should_Increase_Champion_InstructionPointer_After_Stepping()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + 1);
            testedVm.Memory[randomSpace + 11].IndexOfLastChampToWriteHere.Should().Be(1);
        }

        [Test]
        public void Should_Replace_Champion_InstructionPointer_After_Jumping()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.JMP)
                    .With_Modifier(Modifier.A)
                    .With_Operand_A_direct(2)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + 2);
        }

        [Test]
        public void Should_Wrap_Champion_InstructionPointer_When_It_Exceed_Memory_Size()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 99;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.SetupSequence(m => m.Next(0, 100)).Returns(randomSpace)
                .Returns(10);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(0);
        }

        [Test]
        public void Should_Keep_Alternating_Champions_When_No_Failed_Instruction()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + 4);
            testedVm.Memory[randomSpace + 1].Should().Be(testedVm.Memory[randomSpace]);
            testedVm.Memory[randomSpace + 2].Should().Be(testedVm.Memory[randomSpace]);
            testedVm.Memory[randomSpace + 3].Should().Be(testedVm.Memory[randomSpace]);
        }

        [Test]
        public void Should_Create_Separate_Task_At_End_Of_Queue_At_Target_When_New_Task_Should_Spawn()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.SPL)
                    .With_Modifier(Modifier.A)
                    .With_Operand_A_direct(0)
                    .End()
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step();
            testedVm.Step();
            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace);
        }

        [Test]
        public void Should_Not_Add_Current_Task_To_Queue_When_Instruction_Fails()
        {
            Champion c = new ChampionBuilder()
                .AddOpcode(Opcodes.SPL)
                    .With_Modifier(Modifier.A)
                    .With_Operand_A_direct(4)
                    .End()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);

            testedVm.Step(); //exec SPL at 0, queue is [ MOV at 1 , DAT at 4 ]
            testedVm.Step();
            testedVm.Step(); //exec MOV at 1, queue is [ DAT at 4 , MOV at 2 ]
            testedVm.Step();
            testedVm.Step(); //exec DAT at 4, queue is [ MOV at 2 ] (no add of task at 5)
            testedVm.Step();
            testedVm.Step(); //exec MOV at 2, queue is [ MOV at 3 ]
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c);
            testedVm.NextMove.InstructionPointer.Should().Be(randomSpace + 3);
        }

        [Test]
        public void Should_Kill_Champion_When_Its_Last_Task_Dies()
        {
            Champion c = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c3 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Step();

            testedVm.Champions.ElementAt(0).IsAlive.Should().BeFalse();
        }

        [Test]
        public void Should_Remove_Dead_Champions_From_Stepping()
        {
            Champion c = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            Champion c3 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Step();
            testedVm.Step();
            testedVm.Step();

            testedVm.NextMove.Champion.Should().Be(c2);
        }

        [Test]
        public void Should_Declare_Last_Alive_Champion_The_Winner()
        {
            Champion c = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c3 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Step();
            testedVm.Step();

            testedVm.Winner.Should().Be(c3);
        }

        [Test]
        public void Should_Stop_Stepping_When_There_Is_A_Winner()
        {
            Champion c = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c3 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Step();
            testedVm.Step();
            testedVm.Step();

            testedVm.Memory[randomSpace + 20 + 1].Should().NotBe(testedVm.Memory[randomSpace + 20]);
        }

        [Test]
        public void Should_Begin_Without_A_Winner()
        {
            Champion c = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c2 = new ChampionBuilder()
                .AddSimpleDAT(0)
                .Build();
            Champion c3 = new ChampionBuilder()
                .AddOpcode(Opcodes.MOV)
                    .With_Modifier(Modifier.I)
                    .With_Operand_A_direct(0)
                    .With_Operand_B_direct(1)
                    .End()
                .Build();
            int randomSpace = 24;
            Mock<IRandomGenerator> randomMock = new Mock<IRandomGenerator>();
            randomMock.Setup(m => m.Next(0, 100)).Returns(randomSpace);
            MarsVirtualMachine testedVm = new MarsVirtualMachine(100, randomMock.Object);
            testedVm.Separation = 10;
            testedVm.LoadChampion(c);
            testedVm.LoadChampion(c2);
            testedVm.LoadChampion(c3);

            testedVm.Winner.Should().Be(null);
        }
    }
}
