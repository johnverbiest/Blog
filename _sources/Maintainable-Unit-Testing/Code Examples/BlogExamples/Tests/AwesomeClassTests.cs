using AutoFixture;
using AutoFixture.Xunit2;
using BlogExamples.Dependencies;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace BlogExamples.Tests.Conventional
{
    public class AwesomeClassTests
    {
        [Fact]
        public void DoAThing_WhenCalled_ShouldCallDependency1ForResult()
        {
            // Arrange
            var dep1 = A.Fake<IDependency1>();
            var dep2 = A.Fake<IDependency2>();
            var dep3 = A.Fake<IDependency3>();
            var dep4 = A.Fake<IDependency4>();
            var withThis = "Some Text";
            var withThat = 3;

            var sut = new AwesomeClass(dep1, dep2, dep3, dep4);

            // Act
            sut.DoAThing(withThis, withThat);

            // Assert
            A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
        }
        
        [Fact]
        public void DoAThing_WhenCalled_ShouldReturnResultFromDependencyAddedWithThat()
        {
            // Arrange
            var dep1 = A.Fake<IDependency1>();
            var dep2 = A.Fake<IDependency2>();
            var dep3 = A.Fake<IDependency3>();
            var dep4 = A.Fake<IDependency4>();
            var withThis = "Some Text";
            var withThat = 3;
            A.CallTo(() => dep1.Result(withThis)).Returns(5);
            var expectedResult = "8";

            var sut = new AwesomeClass(dep1, dep2, dep3, dep4);

            // Act
            var result = sut.DoAThing(withThis, withThat);

            // Assert
            result.Should().Be(expectedResult);
        }
        
        [Fact]
        public void Lib1Autofixture()
        {
            // Arrange
            var fixture = new Fixture();
            var dep1 = A.Fake<IDependency1>();
            var dep2 = A.Fake<IDependency2>();
            var dep3 = A.Fake<IDependency3>();
            var dep4 = A.Fake<IDependency4>();
            var withThis = fixture.Create<string>();
            var withThat = fixture.Create<int>();

            var sut = new AwesomeClass(dep1, dep2, dep3, dep4);

            // Act
            sut.DoAThing(withThis, withThat);

            // Assert
            A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
        }
        
        [Theory, AutoData]
        public void Lib2AutofixtureXunit2(string withThis, int withThat)
        {
            // Arrange
            var dep1 = A.Fake<IDependency1>();
            var dep2 = A.Fake<IDependency2>();
            var dep3 = A.Fake<IDependency3>();
            var dep4 = A.Fake<IDependency4>();

            var sut = new AwesomeClass(dep1, dep2, dep3, dep4);

            // Act
            sut.DoAThing(withThis, withThat);

            // Assert
            A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
        }
        
        [Theory, UnitTest]
        public void Lib2AutofixtureAutoFakeItEasy(
            [Frozen] IDependency1 dep1, 
            string withThis, 
            int withThat, 
            AwesomeClass sut)
        {
            // Arrange no longer needed:
            // no test specific setup is required for this test

            // Act
            sut.DoAThing(withThis, withThat);

            // Assert
            A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
        }
        
        
        [Theory, UnitTest]
        public void DoAThing_WhenCalled_ExampleWithSetup(
            [Frozen] IDependency1 dep1,
            AwesomeClass sut)
        {
            // Arrange
            var withThis = "<SomeUserInputThatWillLookLikeThis>";
            var withThat = 3; // Also a specific case
            
            // Setup the interface to respond with the value expected from that 
            // interface when calling it.
            A.CallTo(() => dep1.Result(withThis)).Returns(5);
            
            // Act
            var result = sut.DoAThing(withThis, withThat);

            // Assert
            result.Should().Be("8");
        }
    }
}