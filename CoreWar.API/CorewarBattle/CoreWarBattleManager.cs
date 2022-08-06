using Corewar.Core;
using Corewar.Core.Parser;
using Corewar.Core.Random;

namespace CoreWar.API.CorewarBattle
{
    internal class CoreWarBattleManager : ICoreWarBattleManager
    {
        public MarsVirtualMachine? Mars { get; private set; }

        private readonly IRandomGenerator _randomGenerator;
        private readonly IRedcodeToBinary _parser;

        public CoreWarBattleManager(IRandomGenerator randomGenerator, IRedcodeToBinary parser)
        {
            _randomGenerator = randomGenerator;
            _parser = parser;
        }

        public Task<MarsVirtualMachine> CreateBattleAsync(int coreSize, string championOne, string championTwo)
        {
            Mars = new MarsVirtualMachine(coreSize, _randomGenerator);

            Champion c1 = _parser.Parse(championOne);
            Champion c2 = _parser.Parse(championTwo);

            Mars.LoadChampion(c1);
            Mars.LoadChampion(c2);

            return Task.FromResult(Mars);
        }

        public Task<MarsVirtualMachine> StepAsync()
        {
            Mars!.Step();
            return Task.FromResult(Mars);
        }
    }
}
