using Corewar.Core.Enums;
using Corewar.Core.Exceptions;
using Corewar.Core.Memory;
using Corewar.Core.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core
{

    public class MarsVirtualMachine
    {
        private readonly int _size;
        public MemoryCell[] Memory { get; private set; }
        public IEnumerable<Champion> Champions => _champions.Values.AsEnumerable();
        private Dictionary<int, Champion> _champions = new Dictionary<int, Champion>();
        public Champion? Winner { get; private set; }
        public NextMove NextMove => new(NextInstruction.Key, NextInstruction.Value.Peek());
        private KeyValuePair<Champion, Queue<int>> NextInstruction => InstructionPointers.Peek();
        private Queue<KeyValuePair<Champion, Queue<int>>> InstructionPointers { get; set; } = new Queue<KeyValuePair<Champion, Queue<int>>>();
        public int MinimumSpacing { get; internal set; } = 0;
        public int InstructionLimit { get; internal set; } = 0;
        public int Separation { get; internal set; } = 0;

        private readonly IRandomGenerator _random;

        public MarsVirtualMachine(int size, IRandomGenerator random)
        {
            _size = size;
            Memory = new MemoryCell[size];
            for (int i = 0; i < _size; i++)
            {
                Memory[i] = new MemoryCell(Opcodes.DAT, Modifier.F, AddressMode.Immediate, 0, AddressMode.Immediate, 0);
            }
            _random = random;
        }

        internal void LoadChampion(Champion c)
        {
            if (InstructionLimit != 0 && c.Instructions.Length > InstructionLimit)
                throw new ChampionExceedsInstructionLimitException(c);
            int loadIndex;
            if (Separation == 0 || _champions.Count == 0)
                loadIndex = TryGenerateRandomIndexForChampion(c);
            else
                loadIndex = (_champions.Last().Key + Separation) % _size;
            c.Index = _champions.Count;
            _champions.Add(loadIndex, c);
            CopyChampionTo(loadIndex, c);
            InstructionPointers.Enqueue(new(c, new Queue<int>(new[] { loadIndex + c.Origin })));
        }

        private int TryGenerateRandomIndexForChampion(Champion c)
        {
            int randomIndex;
            int tries = 0;
            do
            {
                randomIndex = _random.Next(0, _size);
                tries++;
            } while (ThereIsAlreadyAChampionNear(randomIndex) && tries < 50);
            if (tries == 50)
            {
                throw new CantPositionChampionRandomlyException(c);
            }

            return randomIndex;
        }

        private bool ThereIsAlreadyAChampionNear(int index)
        {
            return _champions.Any(champ =>
                champ.Key <= index
                && (champ.Key + Math.Max(champ.Value.Instructions.Length, MinimumSpacing)) > index
            );
        }

        private void CopyChampionTo(int startIndex, Champion c)
        {
            for (int i = 0; i < c.Instructions.Length; i++)
            {
                Memory[(startIndex + i) % _size] = c.Instructions[i];
                Memory[(startIndex + i) % _size].IndexOfLastChampToWriteHere = _champions.Count - 1;
            }
        }

        internal void Step()
        {
            if (Winner != null)
                return;

            var nextChamp = InstructionPointers.Dequeue();
            var currentInstructionPointer = nextChamp.Value.Dequeue();

            Instruction instruction = new Instruction(Memory, currentInstructionPointer, nextChamp.Key.Index);
            instruction.Execute();

            if (instruction.Success)
            {
                nextChamp.Value.Enqueue((instruction.NextInstructionPointer) % _size);
                if (instruction.NewTaskSpawningAt.HasValue)
                {
                    nextChamp.Value.Enqueue(instruction.NewTaskSpawningAt!.Value);
                }
            }
            if (nextChamp.Value.Count > 0)
                InstructionPointers.Enqueue(nextChamp);
            else 
                nextChamp.Key.IsAlive = false;
            if (_champions.Count(c => c.Value.IsAlive) == 1)
            {
                Winner = _champions.First(c => c.Value.IsAlive).Value;
            }
        }
    }
}
