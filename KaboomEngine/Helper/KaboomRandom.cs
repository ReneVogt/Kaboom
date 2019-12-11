using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Revo.Games.KaboomEngine.Helper
{
    [ExcludeFromCodeCoverage]
    sealed class KaboomRandom : IProvideRandom
    {
        static readonly Random random = new Random();

        public int Next(int max) => (int)(max * (1 - Math.Sqrt(random.NextDouble())));
    }
}
