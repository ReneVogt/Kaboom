using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    sealed class ConstraintsGenerator : IGenerateConstraints
    {
        private static void FillIndices(List<int[]> results, Stack<int> clause, int start, int elements, int needed)
        {
            if (clause.Count == needed)
            {
                results.Add(clause.ToArray());
                return;
            }

            for (int i = start; i < elements; i++)
            {
                clause.Push(i);
                FillIndices(results, clause, i + 1, elements, needed);
                clause.Pop();
            }
        }
        private static int[][] EnumIndices(int elements, int needed)
        {
            List<int[]> results = new List<int[]>();
            Stack<int> clause = new Stack<int>();
            FillIndices(results, clause, 0, elements, needed);
            return results.ToArray();
        }

        public List<Literal[]> GenerateConstraints(int numberOfElements, int expectedNumberOfTrueElements, int[] elementIDs = null)
        {
            if (numberOfElements < 1) throw new ArgumentException("There must be at least one element to constraint.", nameof(numberOfElements));
            if (expectedNumberOfTrueElements < 0 || numberOfElements < expectedNumberOfTrueElements) throw new ArgumentException("The expected number of true elements must be at least zero and at most the total number of elements.", nameof(expectedNumberOfTrueElements));
            if (elementIDs?.Length < numberOfElements) throw new ArgumentOutOfRangeException(nameof(elementIDs), elementIDs.Length, "There must be at least as many element IDs as elements.");

            elementIDs ??= Enumerable.Range(0, numberOfElements).ToArray();

            if (expectedNumberOfTrueElements == 0)
                return Enumerable.Range(0, numberOfElements)
                                 .Select(i => new[] {new Literal(elementIDs[i], false)}).ToList();

            if (expectedNumberOfTrueElements == numberOfElements)
                return Enumerable.Range(0, numberOfElements).Select(i => new[] { new Literal(elementIDs[i], true) }).ToList();

            int needToHaveAtLeastOneTrue = numberOfElements - expectedNumberOfTrueElements + 1;

            var clauses = EnumIndices(numberOfElements, needToHaveAtLeastOneTrue)
                          .Select(indices => indices.Select(i => new Literal(elementIDs[i], true)).ToArray())
                          .ToList();

            int needToHaveAtLeastOneFalse = expectedNumberOfTrueElements + 1;
            clauses.AddRange(EnumIndices(numberOfElements, needToHaveAtLeastOneFalse).Select(indices => indices.Select(i => new Literal(elementIDs[i], false)).ToArray()));

            return clauses;
        }
    }
}
