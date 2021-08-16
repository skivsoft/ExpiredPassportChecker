using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileFormat.PassportData.Tests
{
    [TestClass]
    public class PassportDataStorageTest
    {
        [TestMethod]
        public void Add_ExactSearch_ShouldBeFound()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("1");
 
            // act
            var result = graph.Contains("1");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Add_OtherSearch_ShouldBeNotFound()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("1");
 
            // act
            var result = graph.Contains("2");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Add_TwoCharsExactSearch_ShouldBeFound()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("12");
 
            // act
            var result = graph.Contains("12");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Add_TwoCharsOtherSearch_ShouldBeNotFound()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("12");
 
            // act
            var result = graph.Contains("11");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void AddTwoLines_CheckSecond_ShouldContains()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("6004,270563");
            graph.Add("6004,270564");
            
            // act
            var result = graph.Contains("6004,270564");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void AddTwoLines_CheckWrong_ShouldNotContains()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("6004,270563");
            graph.Add("6004,270564");
            
            // act
            var result = graph.Contains("6004,270565");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void AddTwoLines_CheckSimilarToSecond_ShouldNotContains()
        {
            // arrange
            var graph = new PassportDataStorage();
            graph.Add("563");
            graph.Add("964");
            
            // act
            var result = graph.Contains("564");
            
            // assert
            result.Should().BeFalse();
        }
    }
}