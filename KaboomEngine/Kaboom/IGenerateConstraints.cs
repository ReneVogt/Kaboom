using System.Collections.Generic;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom {
    interface IGenerateConstraints
    {
        List<Literal[]> GenerateConstraints(int numberOfElements, int expectedNumberOfTrueElements, int[] elementIDs = null);
    }
}
