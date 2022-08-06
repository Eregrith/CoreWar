using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corewar.Core.Random
{
    public class RandomGenerator : IRandomGenerator
    {
        private readonly System.Random _random;

        public RandomGenerator()
        {
            _random = new System.Random((Thread.CurrentThread.ManagedThreadId * DateTime.Now.Millisecond) % DateTime.Now.Second);
        }

        public int Next(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
