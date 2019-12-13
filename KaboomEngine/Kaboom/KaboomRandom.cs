using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    [ExcludeFromCodeCoverage]
    sealed class KaboomRandom : IProvideRandom
    {
        static readonly Random random = new Random();

        public int Next(int max) => Math.Max(0, Math.Min(max - 1, (int)(max * Normalize(random.NextDouble()))));
        //static double Normalize(double rand) => 1 - Math.Sqrt(Math.Cos(0.5 * Math.PI * rand));
        static double Normalize(double rand) => 1 - Math.Sqrt(rand);
    }
}
