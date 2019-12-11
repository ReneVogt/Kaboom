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
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\KaboomTests\\KaboomFieldSolverTests\\solvings.xml", "TestCase",
            DataAccessMethod.Sequential)]
        public void Solve_Integration()
        {
            var constraintsGenerator = new ConstraintsGenerator();
            bool randomed = false;
            int random = 0;
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

            string name = TestContext.DataRow["Name"].ToString();
            int x = Convert.ToInt32(TestContext.DataRow["X"].ToString());
            int y = Convert.ToInt32(TestContext.DataRow["Y"].ToString());

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

            int errors = 0;
            foreach(var row in TestContext.DataRow.GetChildRows("TestCase_Solution"))
            {
                randomed = false;
                random = Convert.ToInt32(row["Random"].ToString());
                var field = TestContext.DataRow["Board"].ToString().ToKaboomField(sut);
                var expectedResult = string.Join(Environment.NewLine,
                                                 row["Expected"].ToString().Split('\n').Select(line => line.Trim()))
                                           .Trim();
                sut.Solve(field, x, y);

                string result = field.Serialize().Trim();

                if (result == expectedResult)
                {
                    TestContext.WriteLine($"Test {name}/{random} passed.");
                    continue;
                }
                TestContext.WriteLine($"Test {name}/{random} failed!");
                TestContext.WriteLine("Expected:");
                TestContext.WriteLine(expectedResult);
                TestContext.WriteLine("Instead:");
                TestContext.WriteLine(result);
                errors += 1;
            }

            Assert.AreEqual(0, errors);

            static string GetSolutionString(SatSolution solution) =>
                string.Join(string.Empty,
                            solution.Literals.OrderBy(literal => literal.Var)
                                    .Select(literal => literal.Sense ? $"+{literal.Var}" : $"-{literal.Var}"));

        }
    }
}