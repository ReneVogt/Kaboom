using System;
using System.Linq;
using Com.Revo.Games.KaboomEngine;
using Com.Revo.Games.KaboomEngine.Fakes;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KaboomEngineTests.FieldExtensionsTests
{
    public partial class FieldExtensionsTests
    {
        const int WIDTH = 12;
        const int HEIGHT = 8;
        const int MIDDLE_X = WIDTH >> 1;
        const int MIDDLE_Y = HEIGHT >> 1;
        static readonly IField field = new StubIField
        {
            WidthGet = () => WIDTH,
            HeightGet = () => HEIGHT
        };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetCoordinatesAdjacentTo_NoField_ArgumentNullException()
        {
            IField noField = null;
            // ReSharper disable once IteratorMethodResultIsIgnored
            // ReSharper disable once AssignNullToNotNullAttribute
            noField.GetCoordinatesAdjacentTo(0, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetCoordinatesAdjacentTo_NegativeX_IndexOutOfRangeException()
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            field.GetCoordinatesAdjacentTo(-1, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetCoordinatesAdjacentTo_TooGreatX_IndexOutOfRangeException()
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            field.GetCoordinatesAdjacentTo(WIDTH, 0);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetCoordinatesAdjacentTo_NegativeY_IndexOutOfRangeException()
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            field.GetCoordinatesAdjacentTo(0, -1);
        }
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GetCoordinatesAdjacentTo_TooGreatY_IndexOutOfRangeException()
        {
            // ReSharper disable once IteratorMethodResultIsIgnored
            field.GetCoordinatesAdjacentTo(0, HEIGHT);
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_UpperLeftCorner_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(0, 0).ToArray();
            result.Length.Should().Be(3);
            result.Should().Contain((0, 1));
            result.Should().Contain((1, 0));
            result.Should().Contain((1, 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_UpperRightCorner_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(WIDTH-1, 0).ToArray();
            result.Length.Should().Be(3);
            result.Should().Contain((WIDTH - 2, 0));
            result.Should().Contain((WIDTH - 2, 1));
            result.Should().Contain((WIDTH - 1, 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_LowerLeftCorner_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(0, HEIGHT-1).ToArray();
            result.Length.Should().Be(3);
            result.Should().Contain((0, HEIGHT - 2));
            result.Should().Contain((1, HEIGHT - 2));
            result.Should().Contain((1, HEIGHT - 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_LowerRightCorner_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(WIDTH-1, HEIGHT-1).ToArray();
            result.Length.Should().Be(3);
            result.Should().Contain((WIDTH - 2, HEIGHT - 1));
            result.Should().Contain((WIDTH - 1, HEIGHT - 2));
            result.Should().Contain((WIDTH - 2, HEIGHT - 2));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_LeftBorder_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(0, MIDDLE_Y).ToArray();
            result.Length.Should().Be(5);
            result.Should().Contain((0, MIDDLE_Y - 1));
            result.Should().Contain((1, MIDDLE_Y - 1));
            result.Should().Contain((1, MIDDLE_Y));
            result.Should().Contain((1, MIDDLE_Y + 1));
            result.Should().Contain((0, MIDDLE_Y + 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_TopBorder_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(MIDDLE_X, 0).ToArray();
            result.Length.Should().Be(5);
            result.Should().Contain((MIDDLE_X - 1, 0));
            result.Should().Contain((MIDDLE_X - 1, 1));
            result.Should().Contain((MIDDLE_X, 1));
            result.Should().Contain((MIDDLE_X + 1, 1));
            result.Should().Contain((MIDDLE_X + 1, 0));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_RightBorder_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(WIDTH - 1, MIDDLE_Y).ToArray();
            result.Length.Should().Be(5);
            result.Should().Contain((WIDTH - 1, MIDDLE_Y - 1));
            result.Should().Contain((WIDTH - 2, MIDDLE_Y - 1));
            result.Should().Contain((WIDTH - 2, MIDDLE_Y));
            result.Should().Contain((WIDTH - 2, MIDDLE_Y + 1));
            result.Should().Contain((WIDTH - 1, MIDDLE_Y + 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_BottomBorder_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(MIDDLE_X, HEIGHT - 1).ToArray();
            result.Length.Should().Be(5);
            result.Should().Contain((MIDDLE_X - 1, HEIGHT - 1));
            result.Should().Contain((MIDDLE_X - 1, HEIGHT - 2));
            result.Should().Contain((MIDDLE_X, HEIGHT - 2));
            result.Should().Contain((MIDDLE_X + 1, HEIGHT - 2));
            result.Should().Contain((MIDDLE_X + 1, HEIGHT - 1));
        }
        [TestMethod]
        public void GetCoordinatesAdjacentTo_Middle_CorrectResult()
        {
            var result = field.GetCoordinatesAdjacentTo(MIDDLE_X, MIDDLE_Y).ToArray();
            result.Length.Should().Be(8);
            result.Should().Contain((MIDDLE_X - 1, MIDDLE_Y - 1));
            result.Should().Contain((MIDDLE_X, MIDDLE_Y - 1));
            result.Should().Contain((MIDDLE_X + 1, MIDDLE_Y - 1));
            result.Should().Contain((MIDDLE_X - 1, MIDDLE_Y));
            result.Should().Contain((MIDDLE_X + 1, MIDDLE_Y));
            result.Should().Contain((MIDDLE_X - 1, MIDDLE_Y + 1));
            result.Should().Contain((MIDDLE_X, MIDDLE_Y + 1));
            result.Should().Contain((MIDDLE_X + 1, MIDDLE_Y + 1));
        }
    }
}