using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.SolverFoundation.Solvers;

namespace Com.Revo.Games.KaboomEngine.Kaboom
{
    sealed class ConstraintsGenerator : IGenerateConstraints
    {
        private void FillIndices(List<int[]> results, Stack<int> clause, int start, int elements, int needed)
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
        private int[][] EnumIndices(int elements, int needed)
        {
            List<int[]> results = new List<int[]>();
            Stack<int> clause = new Stack<int>();
            FillIndices(results, clause, 0, elements, needed);
            return results.ToArray();
        }

        [SuppressMessage("Usage", "CA2208:Argumentausnahmen korrekt instanziieren", Justification = "<Ausstehend>")]
        public List<Literal[]> GenerateConstraints(int elements, int expectedSum, int[] elementIDs)
        {
            if (elements == 0 || elements < expectedSum) throw new ArgumentException();

            List<Literal[]> clauses = new List<Literal[]>();
            if (expectedSum == 0)
            {
                clauses.Add(Enumerable.Range(0, elements).Select(i => new Literal(elementIDs[i], false)).ToArray());
                return clauses;
            }

            if (expectedSum == elements)
            {
                clauses.Add(Enumerable.Range(0, elements).Select(i => new Literal(elementIDs[i], true)).ToArray());
                return clauses;
            }


            int needToHaveAtLeastOneTrue = elements - expectedSum + 1;

            clauses.AddRange(EnumIndices(elements, needToHaveAtLeastOneTrue).Select(indices => indices.Select(i => new Literal(elementIDs[i], true)).ToArray()));

            int needToHaveAtLeastOneFalse = expectedSum + 1;
            clauses.AddRange(EnumIndices(elements, needToHaveAtLeastOneFalse).Select(indices => indices.Select(i => new Literal(elementIDs[i], false)).ToArray()));

            return clauses;
        }
    }
}
