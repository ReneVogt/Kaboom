using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    [ExcludeFromCodeCoverage]
    sealed class KaboomSatSolver : IKaboomSatSolver
    {
        public List<SatSolution> Solve(IEnumerable<Literal[]> constraints, int numberOfVariables, int minimumTrueValues, int maximumTrueValues) =>
            (from solution in SatSolver.Solve(new SatSolverParams(), numberOfVariables, constraints)
                             let mines = solution.Literals.Count(literal => literal.Sense)
                             where mines >= minimumTrueValues && mines <= maximumTrueValues
             select solution).ToList();
    }
}
