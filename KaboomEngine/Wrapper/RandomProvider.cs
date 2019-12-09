using System;

namespace Com.Revo.Games.KaboomEngine.Wrapper
{
    sealed class RandomProvider : IProvideRandom
    {
        static readonly Random random = new Random();

        public int Next(int max) => random.Next(max);
    }
}
