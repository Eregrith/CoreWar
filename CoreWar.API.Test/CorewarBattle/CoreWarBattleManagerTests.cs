using Corewar.Core;
using Corewar.Core.Memory;
using Corewar.Core.Parser;
using Corewar.Core.Random;
using CoreWar.API.CorewarBattle;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreWar.API.Test.CorewarBattle
{
    internal class CoreWarBattleManagerTests
    {
        [Test]
        public async Task CreateBattleAsync_Should_Create_New_MarsVirtualMachine_Of_Given_Size()
        {
            int coreSize = 10;
            Mock<IRandomGenerator> mockRandom = new Mock<IRandomGenerator>();
            mockRandom.SetupSequence(m => m.Next(0, coreSize))
                .Returns(0)
                .Returns(5);
            Mock<IRedcodeToBinary> mockParser = new Mock<IRedcodeToBinary>();
            var c1 = new Champion(new MemoryCell());
            var c2 = new Champion(new MemoryCell());
            mockParser.SetupSequence(p => p.Parse(""))
                .Returns(c1)
                .Returns(c2);
            CoreWarBattleManager manager = new CoreWarBattleManager(mockRandom.Object, mockParser.Object);

            await manager.CreateBattleAsync(coreSize, "", "");

            manager.Mars!.Memory.Should().HaveCount(coreSize);
        }

        [Test]
        public async Task CreateBattleAsync_Should_Parse_And_Load_The_Two_Champions()
        {
            int coreSize = 10;
            int randOne = 0;
            int randTwo = 5;
            Mock<IRandomGenerator> mockRandom = new Mock<IRandomGenerator>();
            mockRandom.SetupSequence(m => m.Next(0, coreSize))
                .Returns(randOne)
                .Returns(randTwo);
            Mock<IRedcodeToBinary> mockParser = new Mock<IRedcodeToBinary>();
            var c1 = new Champion(MemoryCell.DAT(2));
            var c2 = new Champion(MemoryCell.DAT(5));
            mockParser.SetupSequence(p => p.Parse(""))
                .Returns(c1)
                .Returns(c2);
            CoreWarBattleManager manager = new CoreWarBattleManager(mockRandom.Object, mockParser.Object);

            var mars = await manager.CreateBattleAsync(coreSize, "", "");

            mars.Should().Be(manager.Mars);
            manager.Mars!.Memory[randOne].Should().Be(c1.Instructions[0]);
            manager.Mars!.Memory[randTwo].Should().Be(c2.Instructions[0]);
        }

        [Test]
        public async Task StepAsync_Should_Execute_Next_VirtualMachine_Step()
        {
            int coreSize = 10;
            int randOne = 0;
            int randTwo = 5;
            Mock<IRandomGenerator> mockRandom = new Mock<IRandomGenerator>();
            mockRandom.SetupSequence(m => m.Next(0, coreSize))
                .Returns(randOne)
                .Returns(randTwo);
            Mock<IRedcodeToBinary> mockParser = new Mock<IRedcodeToBinary>();
            var c1 = new Champion(MemoryCell.DAT(2));
            var c2 = new Champion(MemoryCell.DAT(5));
            mockParser.SetupSequence(p => p.Parse(""))
                .Returns(c1)
                .Returns(c2);
            CoreWarBattleManager manager = new CoreWarBattleManager(mockRandom.Object, mockParser.Object);
            await manager.CreateBattleAsync(coreSize, "", "");

            var mars = await manager.StepAsync();

            mars.Should().Be(manager.Mars);
            manager.Mars!.Champions.ElementAt(0).IsAlive.Should().BeFalse();
        }
    }
}
