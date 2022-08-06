using MediatR;

namespace CoreWar.API.Commands.CreateCoreWarBattle
{
    public class CreateCoreWarBattleCommand : IRequest<CreateCoreWarBattleResponse>
    {
        public int CoreSize { get; set; }
        public string ChampionOne { get; set; }
        public string ChampionTwo { get; set; }

        public override string ToString()
        {
            return $"{{ CoreSize: {CoreSize}, Champion One: {ChampionOne}, Champion Two: {ChampionTwo} }}";
        }
    }
}
