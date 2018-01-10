//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Shouldly;
//using Spect.Net.TestParser.SyntaxTree.DataBlockNode;
//using Spect.Net.TestParser.SyntaxTree.Expressions;

//namespace Spect.Net.TestParser.Test.Parser
//{
//    [TestClass]
//    public class DataBlockTest: ParserTestBed
//    {
//        [TestMethod]
//        public void DataBlockWorksWithEmptyBlock()
//        {
//            // --- Act
//            var visitor = Parse("data end");

//            // --- Assert
//            visitor.Compilation.TestSets.Count.ShouldBe(1);
//            var block = visitor.Compilation.TestSets[0] as DataBlockNode;
//            block.ShouldNotBeNull();
//            block.Span.StartLine.ShouldBe(1);
//            block.Span.StartColumn.ShouldBe(0);
//            block.Span.EndLine.ShouldBe(1);
//            block.Span.EndColumn.ShouldBe(7);

//            block.DataKeywordSpan.StartLine.ShouldBe(1);
//            block.DataKeywordSpan.StartColumn.ShouldBe(0);
//            block.DataKeywordSpan.EndLine.ShouldBe(1);
//            block.DataKeywordSpan.EndColumn.ShouldBe(3);

//            block.EndKeywordSpan.StartLine.ShouldBe(1);
//            block.EndKeywordSpan.StartColumn.ShouldBe(5);
//            block.EndKeywordSpan.EndLine.ShouldBe(1);
//            block.EndKeywordSpan.EndColumn.ShouldBe(7);

//            block.DataMembers.Count.ShouldBe(0);
//        }

//        [TestMethod]
//        public void DataBlockWorksWithSingleValueMember()
//        {
//            // --- Act
//            var visitor = Parse("data ident1: 0x45 end");

//            // --- Assert
//            visitor.Compilation.TestSets.Count.ShouldBe(1);
//            var block = visitor.Compilation.TestSets[0] as DataBlockNode;
//            block.ShouldNotBeNull();
//            block.Span.StartLine.ShouldBe(1);
//            block.Span.StartColumn.ShouldBe(0);
//            block.Span.EndLine.ShouldBe(1);
//            block.Span.EndColumn.ShouldBe(20);

//            block.DataKeywordSpan.StartLine.ShouldBe(1);
//            block.DataKeywordSpan.StartColumn.ShouldBe(0);
//            block.DataKeywordSpan.EndLine.ShouldBe(1);
//            block.DataKeywordSpan.EndColumn.ShouldBe(3);

//            block.EndKeywordSpan.StartLine.ShouldBe(1);
//            block.EndKeywordSpan.StartColumn.ShouldBe(18);
//            block.EndKeywordSpan.EndLine.ShouldBe(1);
//            block.EndKeywordSpan.EndColumn.ShouldBe(20);

//            block.DataMembers.Count.ShouldBe(1);
//            var vm = block.DataMembers[0] as ValueMemberNode;
//            vm.ShouldNotBeNull();
//            vm.IdSpan.StartLine.ShouldBe(1);
//            vm.IdSpan.StartColumn.ShouldBe(5);
//            vm.IdSpan.EndLine.ShouldBe(1);
//            vm.IdSpan.EndColumn.ShouldBe(10);
//            vm.Id.ShouldBe("IDENT1");
//            vm.Expr.ShouldNotBeNull();
//            vm.Expr.ShouldBeOfType<LiteralNode>();
//        }

//        [TestMethod]
//        public void DataBlockWorksWithMultipleValueMembers()
//        {
//            // --- Act
//            var visitor = Parse(@"data 
//                                    ident1: 0x45,
//                                    ident2: %111_0000_1111,
//                                    ident3: 12345
//                                  end");

//            // --- Assert
//            visitor.Compilation.TestSets.Count.ShouldBe(1);
//            var block = visitor.Compilation.TestSets[0] as DataBlockNode;
//            block.ShouldNotBeNull();
//            block.DataMembers.Count.ShouldBe(3);
//            var vm = block.DataMembers[0] as ValueMemberNode;
//            vm.ShouldNotBeNull();
//            vm.Id.ShouldBe("IDENT1");
//            vm.Expr.ShouldNotBeNull();
//            vm.Expr.ShouldBeOfType<LiteralNode>();
//        }


//    }
//}