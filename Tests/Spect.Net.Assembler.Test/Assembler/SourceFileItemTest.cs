using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Spect.Net.Assembler.Assembler;
// ReSharper disable ObjectCreationAsStatement

namespace Spect.Net.Assembler.Test.Assembler
{
    [TestClass]
    public class SourceFileItemTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructionWithNullFilenameFails()
        {
            // --- Act
            new SourceFileItem(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructionWithEmptyFilenameFails()
        {
            // --- Act
            new SourceFileItem("  \t \r \n   ");
        }

        [TestMethod]
        public void ConstructionWorksAsExpected()
        {
            // --- Act
            var si = new SourceFileItem("myitem.z80asm");

            // --- Assert
            si.ShouldNotBeNull();
            si.Filename.ShouldBe("myitem.z80asm");
            si.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void IncludeWorksWithSingleItem()
        {
            // --- Arrange
            var si = new SourceFileItem("myitem.z80asm");

            // --- Act
            var child = new SourceFileItem("mychild");
            si.Include(child);

            // --- Assert
            si.ShouldNotBeNull();
            si.Filename.ShouldBe("myitem.z80asm");
            si.Parent.ShouldBeNull();
            si.Includes.Count.ShouldBe(1);
            si.Includes.ShouldContain(child);
            child.Parent.ShouldBe(si);
        }

        [TestMethod]
        public void IncludeWorksWithMultipleItems()
        {
            // --- Arrange
            var si = new SourceFileItem("myitem.z80asm");

            // --- Act
            var child1 = new SourceFileItem("mychild1");
            si.Include(child1);
            var child2 = new SourceFileItem("mychild2");
            si.Include(child2);

            // --- Assert
            si.Parent.ShouldBeNull();
            si.Includes.Count.ShouldBe(2);
            si.Includes.ShouldContain(child1);
            child1.Parent.ShouldBe(si);
            si.Includes.ShouldContain(child2);
            child2.Parent.ShouldBe(si);
        }

        [TestMethod]
        public void IncludeDoesNotAddRepeatedItem()
        {
            // --- Arrange
            var si = new SourceFileItem("myitem.z80asm");

            // --- Act
            var child1 = new SourceFileItem("mychild");
            var result1 = si.Include(child1);
            var child2 = new SourceFileItem("mychild");
            var result2 = si.Include(child2);

            // --- Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
            si.Parent.ShouldBeNull();
            si.Includes.Count.ShouldBe(1);
            si.Includes.ShouldContain(child1);
            child1.Parent.ShouldBe(si);
            si.Includes.ShouldNotContain(child2);
            child2.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void IncludeDoesNotAddItemWithItsOwnName()
        {
            // --- Arrange
            var si = new SourceFileItem("myitem.z80asm");

            // --- Act
            var child1 = new SourceFileItem("mychild");
            var result1 = si.Include(child1);
            var child2 = new SourceFileItem("myitem.z80asm");
            var result2 = si.Include(child2);

            // --- Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
            si.Parent.ShouldBeNull();
            si.Includes.Count.ShouldBe(1);
            si.Includes.ShouldContain(child1);
            child1.Parent.ShouldBe(si);
            si.Includes.ShouldNotContain(child2);
            child2.Parent.ShouldBeNull();
        }

        [TestMethod]
        public void IncludeDoesNotAddItemWithParentName()
        {
            // --- Arrange
            var si = new SourceFileItem("myitem.z80asm");

            // --- Act
            var child1 = new SourceFileItem("mychild");
            var result1 = si.Include(child1);
            var child2 = new SourceFileItem("myitem.z80asm");
            var result2 = child1.Include(child2);

            // --- Assert
            result1.ShouldBeTrue();
            result2.ShouldBeFalse();
            si.Parent.ShouldBeNull();
            si.Includes.Count.ShouldBe(1);
            si.Includes.ShouldContain(child1);
            child1.Parent.ShouldBe(si);
            si.Includes.ShouldNotContain(child2);
            child2.Includes.ShouldNotContain(child2);
            child2.Parent.ShouldBeNull();
        }
    }
}
