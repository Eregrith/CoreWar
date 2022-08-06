using Corewar.Core;

namespace CoreWar.API.Commands.Step
{
    public class StepResponse
    {
        public MarsVirtualMachine Mars { get; private set; }

        public StepResponse(MarsVirtualMachine mars)
        {
            Mars = mars;
        }
    }
}
