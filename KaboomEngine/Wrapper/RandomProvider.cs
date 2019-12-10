using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Revo.Games.KaboomEngine.Wrapper
{
    [ExcludeFromCodeCoverage]
    sealed class RandomProvider : IProvideRandom
    {
        static readonly Random random = new Random();

        public int Next(int max) => random.Next(max);
    }
}
