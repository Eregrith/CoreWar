using Corewar.Core;
using CoreWar.API.Commands.Step;
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

namespace CoreWar.API.Test.Commands.Step
{
    internal class StepCommandHandlerTests
    {

        [Test]
        public async Task Should_Call_Step_On_BattleHandler_And_Return_Status()
        {
            MarsVirtualMachine fakeMars = new MarsVirtualMachine(1, null);
            Mock<ICoreWarBattleManager> mockManager = new Mock<ICoreWarBattleManager>();
            mockManager.Setup(m => m.StepAsync()).Returns(Task.FromResult(fakeMars));
            StepCommandHandler handler = new StepCommandHandler(mockManager.Object);

            StepResponse result = await handler.Handle(new StepCommand(), default(CancellationToken));

            result.Mars.Should().Be(fakeMars);
        }
    }
}
