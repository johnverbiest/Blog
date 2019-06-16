---
title: Maintainable Unit Testing
categories: [code, unit testing]
tags: [code, unit testing, test, xunit, fakeiteasy, autofixture, maintainable, tdd]
---
![Test Pattern](/assets/test-pattern.png#rightIcon)
I'm a big fan of Test Driven Development. I try to push my enthusiasm to my colleagues, but they usually find reasons not to do it. One of these reasons is maintainability, and for a while, I could not disagree with them.

After a visit to Techorama in Antwerp in 2017, I found some insights in unit testing that changed the way I write code and unit tests. Here's my story.

# Defining the problem
When you write code the TDD way, you follow these steps:

1. Write a (failing) unit test for something your Unit has to do
2. Write the implementation in your Unit, with the least amount of effort.
3. Refactor and repeat

Following these steps lead to hundreds of Unit Tests throughout your code base. And this is good until you have to change something.

## Issue 1: Dependency Change
Imagine this: you have a great Class with 20 useful methods, thoroughly tested, and you want to add some functionality. At the moment, your Class looks a little like this:

```cs

public class AwesomeClass: IAmAnAwesomeClass
{
    private readonly IDependency1 _dep1;
    private readonly IDependency2 _dep2;

    public AwesomeClass(IDependency1 dep1, IDependency2 dep2)
    {
        _dep1 = dep1;
        _dep2 = dep2;
    }

    public string DoAThing(string withThis, int withThat)
    {
        var someResult = _dep1.Result(withThis);
        someResult += withThat;
        return someResult.ToString();
    }

    public string DoAnotherThing(string withThis, int withThat)
    {
        var someResult = _dep2.Result(withThis);
        someResult += withThat;
        return someResult.ToString();
    }

    // And more and more and more...

}

```

And a typical test looks like this:

```cs

// I am using XUnit as test framework, and FakeItEasy as Mock/Stub generator.

public class AwesomeClassTests
{
    [Fact]
    public void DoAThing_WhenCalled_ShouldCallDependency1ForResult()
    {
        // Arrange
        var dep1 = A.Fake<IDependency1>();
        var dep2 = A.Fake<IDependency2>();
        var withThis = "Some Text";
        var withThat = 3;

        var sut = new AwesomeClass(dep1, dep2);

        // Act
        sut.DoAThing(withThis, withThat)

        // Assert
        A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
    }
}

```

When you want to implement the new feature, however, you need to add a new dependency in the constructor of the Class, and now TDD Hell emerges. Instantly you have all your old tests broken, and you can start fixing this one by one. Fixing all your tests is a tedious, dangerous, and above all boring job. We're developers: we don't like boring work. We need to address this.
