using Corewar.Core;

namespace CoreWar.API.CorewarBattle
{
    internal interface ICoreWarBattleManager
    {
        Task<MarsVirtualMachine> CreateBattleAsync(int coreSize, string championOne, string championTwo);
        Task<MarsVirtualMachine> StepAsync();
    }
}
