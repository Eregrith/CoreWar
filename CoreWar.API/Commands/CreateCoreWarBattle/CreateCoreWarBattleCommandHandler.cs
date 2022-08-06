using CoreWar.API.CorewarBattle;
using MediatR;

namespace CoreWar.API.Commands.CreateCoreWarBattle
{
    internal class CreateCoreWarBattleCommandHandler : IRequestHandler<CreateCoreWarBattleCommand, CreateCoreWarBattleResponse>
    {
        private readonly ICoreWarBattleManager _battleManager;

        public CreateCoreWarBattleCommandHandler(ICoreWarBattleManager battleManager)
        {
            _battleManager = battleManager;
        }

        public async Task<CreateCoreWarBattleResponse> Handle(CreateCoreWarBattleCommand request, CancellationToken cancellationToken)
        {
            var mars = await _battleManager.CreateBattleAsync(request.CoreSize, request.ChampionOne, request.ChampionTwo);
            return new CreateCoreWarBattleResponse(mars);
        }
    }
}
