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
    }
}