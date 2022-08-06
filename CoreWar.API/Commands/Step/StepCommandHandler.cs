using CoreWar.API.CorewarBattle;
using MediatR;

namespace CoreWar.API.Commands.Step
{
    internal class StepCommandHandler : IRequestHandler<StepCommand, StepResponse>
    {
        private ICoreWarBattleManager _manager;

        public StepCommandHandler(ICoreWarBattleManager manager)
        {
            _manager = manager;
        }

        public async Task<StepResponse> Handle(StepCommand request, CancellationToken cancellationToken)
        {
            var mars = await _manager.StepAsync();

            return new StepResponse(mars);
        }
    }
}
