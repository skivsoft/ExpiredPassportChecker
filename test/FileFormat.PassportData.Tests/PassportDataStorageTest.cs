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
            var storage = new PassportDataStorage();
            storage.Add("1");
 
            // act
            var result = storage.Contains("1");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Add_OtherSearch_ShouldBeNotFound()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("1");
 
            // act
            var result = storage.Contains("2");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void Add_TwoCharsExactSearch_ShouldBeFound()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("12");
 
            // act
            var result = storage.Contains("12");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Add_TwoCharsOtherSearch_ShouldBeNotFound()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("12");
 
            // act
            var result = storage.Contains("11");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void AddTwoLines_CheckSecond_ShouldContains()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("6004,270563");
            storage.Add("6004,270564");
            
            // act
            var result = storage.Contains("6004,270564");
            
            // assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void AddTwoLines_CheckWrong_ShouldNotContains()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("6004,270563");
            storage.Add("6004,270564");
            
            // act
            var result = storage.Contains("6004,270565");
            
            // assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void AddTwoLines_CheckSimilarToSecond_ShouldNotContains()
        {
            // arrange
            var storage = new PassportDataStorage();
            storage.Add("563");
            storage.Add("964");
            
            // act
            var result = storage.Contains("564");
            
            // assert
            result.Should().BeFalse();
        }
    }
}