using Corewar.Core;
using CoreWar.API.Commands.CreateCoreWarBattle;
using CoreWar.API.CorewarBattle;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreWar.API.Test.Commands.CreateCoreWarBattle
{
    internal class CreateCoreWarBattleCommandHandlerTests
    {
        [Test]
        public async Task Should_Create_Battle_With_Request_Parameters_And_Return_Battle_Status()
        {
            CreateCoreWarBattleCommand request = new CreateCoreWarBattleCommand
            {
                CoreSize = 100,
                ChampionOne = "toto",
                ChampionTwo = "titi"
            };
            MarsVirtualMachine fakeMars = new MarsVirtualMachine(1, null);
            Mock<ICoreWarBattleManager> mockManager = new Mock<ICoreWarBattleManager>();
            mockManager.Setup(m => m.CreateBattleAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(fakeMars));
            CreateCoreWarBattleCommandHandler handler = new CreateCoreWarBattleCommandHandler(mockManager.Object);

            CreateCoreWarBattleResponse result = await handler.Handle(request, default(CancellationToken));

            mockManager.Verify(m => m.CreateBattleAsync(request.CoreSize, request.ChampionOne, request.ChampionTwo));
            result.Mars.Should().Be(fakeMars);
        }
    }
}
