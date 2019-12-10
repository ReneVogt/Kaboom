using System;
using System.Diagnostics.CodeAnalysis;

namespace Com.Revo.Games.KaboomEngine.Helper
{
    [ExcludeFromCodeCoverage]
    sealed class KaboomRandom : IProvideRandom
    {
        static readonly Random random = new Random();

        public int Next(int max) => random.Next(max);
        //public static double NextGaussian(IProvideRandom r, double mu = 0, double sigma = 1)
        //{
        //    var u1 = r.NextDouble();
        //    var u2 = r.NextDouble();

        //    var rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
        //                          Math.Sin(2.0 * Math.PI * u2);
        //    return rand_std_normal;
        //    //var rand_normal = mu + sigma * rand_std_normal;

        //    //return rand_normal;

        //}
    }
}
