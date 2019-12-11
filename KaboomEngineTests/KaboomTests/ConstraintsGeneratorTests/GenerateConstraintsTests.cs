using System;
using System.Collections.Generic;
using System.Linq;
using Com.Revo.Games.KaboomEngine.Kaboom;
using Microsoft.SolverFoundation.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KaboomEngineTests.KaboomTests.ConstraintsGeneratorTests
{
    public partial class ConstraintsGeneratorTests
    {
        static string SortTestResult(IEnumerable<Literal[]> testResult) =>
            string.Join("|",
                        testResult.Select(line =>
                                              string.Join(string.Empty,
                                                          line.OrderBy(literal => literal.Var)
                                                              .Select(literal => literal.Sense ? $"+{literal.Var}" : $"-{literal.Var}")))
                                  .OrderBy(line => line));

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateConstraints_TooFewElements_ArgumentException()
        {
            var sut = new ConstraintsGenerator();
            sut.GenerateConstraints(0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateConstraints_TooFewExpectedElements_ArgumentException()
        {
            var sut = new ConstraintsGenerator();
            sut.GenerateConstraints(10, -1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GenerateConstraints_TooManyExpectedElements_ArgumentException()
        {
            var sut = new ConstraintsGenerator();
            sut.GenerateConstraints(10, 11);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void GenerateConstraints_TooFewIDs_ArgumentOutOfRangeException()
        {
            var sut = new ConstraintsGenerator();
            sut.GenerateConstraints(4, 1, new []{1,2,3});
        }
        [TestMethod]
        public void GenerateConstraints_1_0_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(1, 0));
            Assert.AreEqual("-0", result);
        }
        [TestMethod]
        public void GenerateConstraints_1_1_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(1, 1));
            Assert.AreEqual("+0", result);
        }
        [TestMethod]
        public void GenerateConstraints_2_0_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(2, 0));
            Assert.AreEqual("-0|-1", result);
        }
        [TestMethod]
        public void GenerateConstraints_2_1_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(2, 1));
            Assert.AreEqual("+0+1|-0-1", result);
        }
        [TestMethod]
        public void GenerateConstraints_2_2_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(2, 2));
            Assert.AreEqual("+0|+1", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_0_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 0));
            Assert.AreEqual("-0|-1|-2|-3|-4", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_1_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 1));
            Assert.AreEqual("+0+1+2+3+4|-0-1|-0-2|-0-3|-0-4|-1-2|-1-3|-1-4|-2-3|-2-4|-3-4", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_2_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 2));
            Assert.AreEqual("+0+1+2+3|+0+1+2+4|+0+1+3+4|+0+2+3+4|+1+2+3+4|-0-1-2|-0-1-3|-0-1-4|-0-2-3|-0-2-4|-0-3-4|-1-2-3|-1-2-4|-1-3-4|-2-3-4", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_3_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 3));
            Assert.AreEqual("+0+1+2|+0+1+3|+0+1+4|+0+2+3|+0+2+4|+0+3+4|+1+2+3|+1+2+4|+1+3+4|+2+3+4|-0-1-2-3|-0-1-2-4|-0-1-3-4|-0-2-3-4|-1-2-3-4", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_4_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 4));
            Assert.AreEqual("+0+1|+0+2|+0+3|+0+4|+1+2|+1+3|+1+4|+2+3|+2+4|+3+4|-0-1-2-3-4", result);
        }
        [TestMethod]
        public void GenerateConstraints_5_5_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(5, 5));
            Assert.AreEqual("+0|+1|+2|+3|+4", result);
        }
        [TestMethod]
        public void GenerateConstraints_4_3_WithIDs_Clauses()
        {
            var sut = new ConstraintsGenerator();
            var result = SortTestResult(sut.GenerateConstraints(4, 3, new []{1,3,5,7}));
            Assert.AreEqual("+1+3|+1+5|+1+7|+3+5|+3+7|+5+7|-1-3-5-7", result);
        }
        [TestMethod]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML",
            "|DataDirectory|\\KaboomTests\\ConstraintsGeneratorTests\\SumSolverTests.xml", "SumSolution",
            DataAccessMethod.Sequential)]
        public void GenerateConstraints_SolverIntegration()
        {
            int numberOfElements = Convert.ToInt32(TestContext.DataRow["NumberOfElements"]);
            int expectedTrueElements = Convert.ToInt32(TestContext.DataRow["ExpectedTrueElements"]);
            string expectedSolution = string.Join(Environment.NewLine, 
                                                  TestContext.DataRow["Solution"].ToString().Split('\n').Select(line => line.Trim())).Trim();

            var sut = new ConstraintsGenerator();
            var constraints = sut.GenerateConstraints(numberOfElements, expectedTrueElements);
            string solutions = string.Join(Environment.NewLine, SatSolver.Solve(new SatSolverParams(), numberOfElements, constraints)
                                                                         .Select(solution => string.Join(
                                                                                     string.Empty, solution.Literals.OrderBy(literal => literal.Var)
                                                                                                           .Select(literal => literal.Sense
                                                                                                                                  ? $"+{literal.Var}"
                                                                                                                                  : $"-{literal.Var}")))
                                                                         .OrderBy(line => line));

            if (solutions == expectedSolution) return;
            TestContext.WriteLine($"Test case with {numberOfElements} elements and {expectedTrueElements} expected true elements failed:");
            TestContext.WriteLine("Expected result:");
            TestContext.WriteLine(expectedSolution);
            TestContext.WriteLine("Instead:");
            TestContext.WriteLine(solutions);

            Assert.Fail("Solutions did not match expectation!");
        }
    }
}