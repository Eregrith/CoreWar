using Corewar.Core;

namespace CoreWar.API.Commands.CreateCoreWarBattle
{
    public class CreateCoreWarBattleResponse
    {
        public MarsVirtualMachine Mars { get; set; }

        public CreateCoreWarBattleResponse(MarsVirtualMachine mars)
        {
            Mars = mars;
        }
    }
}
