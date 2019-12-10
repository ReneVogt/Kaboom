using System;
using Com.Revo.Games.KaboomEngine.Kaboom;
using Com.Revo.Games.KaboomEngine.Kaboom.Fakes;
using Com.Revo.Games.KaboomEngine.Wrapper.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KaboomEngineTests.KaboomTests.KaboomFieldSolverTests
{
    public partial class KaboomFieldSolverTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Solve_NoField_ArgumentNullException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom());
            // ReSharper disable once AssignNullToNotNullAttribute
            sut.Solve(null, 0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_NegativeX_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, -1, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_TooGreatX_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 10, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_NegativeY_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 0, -1);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Solve_TooGreatY_ArgumentOutOfRangeException()
        {
            var sut = new KaboomFieldSolver(new StubIGenerateConstraints(), new StubIProvideRandom());
            KaboomField field = new KaboomField(10, 10, 5, sut);
            sut.Solve(field, 0, 10);
        }
    }
}