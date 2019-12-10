using System;
using System.Linq;
using Com.Revo.Games.KaboomEngine.Helper.Fakes;
using Com.Revo.Games.KaboomEngine.Kaboom;
using Com.Revo.Games.KaboomEngine.Kaboom.Fakes;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KaboomEngineTests.KaboomTests.KaboomFieldSolverTests
{
    public partial class KaboomFieldSolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Solve_NoField_ArgumentNullException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom(), new StubIKaboomSatSolver());
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Solve(null, 0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_NegativeX_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom(), new StubIKaboomSatSolver());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, -1, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_TooGreatX_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom(), new StubIKaboomSatSolver());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 10, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_NegativeY_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom(), new StubIKaboomSatSolver());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 0, -1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_TooGreatY_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom(), new StubIKaboomSatSolver());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 0, 10);
        }

        [TestMethod]
        public void Solve_Integration_001_00()
        {
            const string input = @"
5 5 3
11...
.....
.....
.....
.....";
            const string output = @"
5 5 3
110_.
!___.
.....
.....
.....";
            TestSolver(input, output, 2, 0, 0);
        }
        [TestMethod]
        public void Solve_Integration_001_01()
        {
            const string input = @"
5 5 3
11...
.....
.....
.....
.....";
            const string output = @"
5 5 3
111?.
??_?.
.....
.....
.....";
            TestSolver(input, output, 2, 0, 1);
        }
        [TestMethod]
        public void Solve_Integration_001_02()
        {
            const string input = @"
5 5 3
11...
.....
.....
.....
.....";
            const string output = @"
5 5 3
112?.
??_?.
.....
.....
.....";
            TestSolver(input, output, 2, 0, 2);
        }
        [TestMethod]
        public void Solve_Integration_001_03()
        {
            const string input = @"
5 5 3
11...
.....
.....
.....
.....";
            const string output = @"
5 5 3
113!.
_!_!.
.....
.....
.....";
            TestSolver(input, output, 2, 0, 3);
        }
        [TestMethod]
        public void Solve_Integration_002_00()
        {
            const string input = @"
4 3 5
2?..
??..
....
";
            const string output = @"
4 3 5
22_.
!!_.
....";
            TestSolver(input, output, 1, 0, 0);
        }
        [TestMethod]
        public void Solve_Integration_002_01()
        {
            const string input = @"
4 3 5
2?..
??..
....
";
            const string output = @"
4 3 5
23?.
!!?.
....";
            TestSolver(input, output, 1, 0, 1);
        }
        [TestMethod]
        public void Solve_Integration_003_00()
        {
            const string input = @"
5 4 5
22_..
!!_..
.....
.....
";
            const string output = @"
5 4 5
221_.
!!__.
.....
.....";
            TestSolver(input, output, 2, 0, 0);
        }
        [TestMethod]
        public void Solve_Integration_003_01()
        {
            const string input = @"
5 4 5
22_..
!!_..
.....
.....
";
            const string output = @"
5 4 5
222?.
!!_?.
.....
.....";
            TestSolver(input, output, 2, 0, 1);
        }
        [TestMethod]
        public void Solve_Integration_003_02()
        {
            const string input = @"
5 4 5
22_..
!!_..
.....
.....
";
            const string output = @"
5 4 5
223!.
!!_!.
.....
.....";
            TestSolver(input, output, 2, 0, 2);
        }

        void TestSolver(string input, string expectedOutput, int x, int y, int random)
        {
            var constraintsGenerator = new ConstraintsGenerator();
            bool randomed = false;
            var randomProvider = new StubIProvideRandom
            {
                NextInt32 = max =>
                {
                    if (randomed) return 0;
                    randomed = true;
                    TestContext.WriteLine($"Number of solutions: {max}");
                    return random;
                }
            };
            var solver = new KaboomSatSolver();
            var satSolver = new StubIKaboomSatSolver
            {
                SolveIEnumerableOfLiteralArrayInt32Int32Int32 =
                    (constraints, count, min, max) =>
                        solver.Solve(constraints, count, min, max)
                              .OrderBy(GetSolutionString)
                              .ToList()
            };
            var sut = new KaboomFieldSolver(constraintsGenerator, 
                                            randomProvider, satSolver);
            KaboomField field = input.ToKaboomField(sut);
            sut.Solve(field, x, y);
            Assert.AreEqual(expectedOutput.Trim(), field.Serialize().Trim());

            static string GetSolutionString(SatSolution solution) =>
                string.Join(string.Empty,
                            solution.Literals.OrderBy(literal => literal.Var)
                                    .Select(literal => literal.Sense ? $"+{literal.Var}" : $"-{literal.Var}"));
        }
    }
}