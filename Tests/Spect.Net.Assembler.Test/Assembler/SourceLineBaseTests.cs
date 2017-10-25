using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.SyntaxTree;

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class SourceLineBaseTests
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("Hello world", false)]
        [DataRow("; a comment todo: test me", true)]
        public void DefinesTask_WhenTheCommentHasOrDoesNotHaveText_ShouldReturnTheCorrectValue(string comment, bool expectedValue)
        {
            var sut = CreateSut(comment);
            expectedValue.ShouldBe(sut.DefinesTask);
        }

        [TestMethod]
        [DataRow("todo: this is a test", "todo: this is a test")]
        [DataRow("not this text before todo: this is a test", "todo: this is a test")]
        public void TaskDescription_WhenACommentContainsAToD_ShouldReturnTHeLineConentStartingFromTheToDo(string comment, string expectedValue)
        {
            var sut = CreateSut(comment);
            expectedValue.ShouldBe(sut.TaskDescription);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void TaskDescription_WhenACommentContainsNoData_ShouldReturnNullAndNotThrow(string comment)
        {
            var sut = CreateSut(comment);
            sut.TaskDescription.ShouldBeNull();
        }

        private SourceLineBase CreateSut(string comment)
        {
            return new StubSourceLineBase() { Comment = comment };
        }

        private class StubSourceLineBase : SourceLineBase
        {
        }
    }
}