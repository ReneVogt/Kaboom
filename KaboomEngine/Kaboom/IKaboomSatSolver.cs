using System.Collections.Generic;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom {
    interface IKaboomSatSolver
    {
        List<SatSolution> Solve(IEnumerable<Literal[]> constraints, int numberOfVariables, int minimumTrueValues, int maximumTrueValues);
    }
}
