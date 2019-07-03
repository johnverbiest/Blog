---
title: Maintainable Unit Testing
categories: [code, unit testing]
tags: [code, unit testing, test, xunit, fakeiteasy, autofixture, maintainable, tdd]
---
![Test Pattern](/assets/test-pattern.png#rightIcon)
I'm a big fan of Test Driven Development. I try to push my enthusiasm to my colleagues, but they usually find reasons not to do it. One of these reasons is maintainability, and for a while, I could not disagree with them.

After a visit to Techorama in Antwerp in 2017, I found some insights in unit testing that changed the way I write code and unit tests. Here's my story.

<!--more-->

# Defining the problem
When you write code the TDD way, you follow these steps:

1. Write a (failing) unit test for something your Unit has to do
2. Write the implementation in your Unit, with the least amount of effort
3. Refactor and repeat

Following these steps leads to hundreds of Unit Tests throughout your code base. And this is good until you have to change something.

## Issue 1: Dependency Change
Imagine this: you have a great class with 20 useful methods, thoroughly tested, and you want to add some functionality. At the moment, your class looks a little like this:

```cs
public class AwesomeClass : IAwesomeClass
{
    private readonly IDependency1 _dep1;
    private readonly IDependency2 _dep2;
    private readonly IDependency3 _dep3;
    private readonly IDependency4 _dep4;

    public AwesomeClass(IDependency1 dep1, IDependency2 dep2, IDependency3 dep3, IDependency4 dep4)
    {
        _dep1 = dep1;
        _dep2 = dep2;
        _dep3 = dep3;
        _dep4 = dep4;
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
    
    // And many more thing to come
}
```

And a typical test looks like this:

```cs
using FakeItEasy;
using FluentAssertions;
using Xunit;

// Fluff omitted

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

// Fluff omitted
```

When you want to implement the new feature, however, you need to add a new dependency in the constructor of the Class, and now TDD Hell emerges. Instantly you have all your old tests broken, and you can start fixing these one by one. Fixing all your tests is a tedious, dangerous, and, above all, boring job. We're developers: we don't like boring work. We need to address this.


## Issue 2: Focus on what matters
Take a look at this test again:

```cs
using FakeItEasy;
using FluentAssertions;
using Xunit;

// Fluff omitted

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

// Fluff omitted
```

As you can see, there are 7 lines of code in the "arrange" phase, 1 line of code in the "act" phase, and one line of code in the "assert phase". A test that is heavy on the "arrange" phase is harder to read. When you want to know the meaning of the test only by looking at the code, some questions may arise.
1. What are dependencies 2 to 4 used for? 
2. Is it essential that the value of "withThis" is "Some Text" specifically?
3. Is it necessary that the value of "withThat" is "3" specifically?
4. Will the test fail if I change any of the previous values? 

To address these issues, I made some ground rules for myself:
1. Tests should be as short as possible
2. If the value of some variable is not important, it should be clear.
3. If the value of a variable is important, set it in the "arrange" phase.

These rules produce short unit tests, where it is clear to see what is essential to run this test and what this test is actually about. However, how do we do this?

# Introducing another way of testing
## Library 1: Autofixture
If the value of some specific variable is not crucial for a test, It should be random.
To make this happen, You can use a library called [AutoFixture](https://github.com/AutoFixture/AutoFixture). This library can generate random values for whatever concrete type you ask. Even complex and nested types are no issue for AutoFixture. 

```cs
[Fact]
public void Lib1Autofixture()
{
    // Arrange
    var dep1 = A.Fake<IDependency1>();
    var dep2 = A.Fake<IDependency2>();
    var dep3 = A.Fake<IDependency3>();
    var dep4 = A.Fake<IDependency4>();
    var fixture = new Fixture();
    var withThis = fixture.Create<string>();
    var withThat = fixture.Create<int>();

    var sut = new AwesomeClass(dep1, dep2, dep3, dep4);

    // Act
    sut.DoAThing(withThis, withThat);

    // Assert
    A.CallTo(() => dep1.Result(withThis)).MustHaveHappenedOnceExactly();
}
```

## Library 2: Autofixture.Xunit2
With Autofixture.Xunit2, you can inject random values for concrete classes in your method parameters. No need to create a Fixture manually and use it. Using the Theory attribute you instruct Xunit to request some data from the AuthoData Attribute.
```cs
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
```
As you can see, the unit test starts to get cleaner and is becoming more readable.

## Library 3: AutoFixture.AutoFakeItEasy
If you were to ask for an instance of the "AwesomeClass" in your test method parameters, you would get an exception. AutoFixture is designed to only create implementations of concrete classes, but the constructor of the "AwesomeClass" needs 4 implementations of interfaces. However, we can customize AutoFixture so that it's capable of providing implementations for interfaces via the AutoFakeItEasy library.

To facilitate this, we have to create a new attribute, inheriting from the AutoData attribute. I like to call this my "UnitTestAttribute" as all this thing does is provide implementations for unit tests.

```cs
/// <summary>
/// This is the base test attribute, used in almost all the in-memory unit tests.
/// Whenever a special configuration is required (example: for implementation tests)
/// you can use the static CreateBasicFixture as a base.
/// </summary>
public class UnitTestAttribute : AutoDataAttribute
{
    public UnitTestAttribute() : base(CreateBasicFixture)
    { /* Leave empty */}

    /// <summary>
    /// Creates the fixture for a basic in-memory unit test, to be re-used by other TestAttributes
    /// </summary>
    /// <returns>Configured fixture</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static IFixture CreateBasicFixture()
    {
        var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
        return fixture;
    }
}
```
The magic happens in the ```new Fixture().Customize(new AutoFakeItEasyCustomization())``` part of the code. It instructs AutoFixture to customize its inner workings and request a fake implementation of an interface whenever it runs into one to create an object. Instead of throwing an exception, it now uses FakeItEasy to build a mock instance of that object. 

Our test now looks like this:
```cs
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
```
Few things to note: 

First, there is no need for an "arrange" phase. Everything that required setup is of no real importance to the scope of this test and is not included. Now our test is very readable. 

Secondly, we need the dependency ```dep1``` in our "assert" phase of the test. We can request this in our method parameters. To make sure the instance requested by the method is the same as the instance injected in the AwesomeClass, we add the attribute ```[Frozen]``` to it. Frozen works as follows: when the test gets started, the ```UnitTest``` attribute runs through all the method parameters one by one. Each time it needs an object, it requests AutoFixture to create one. However, when the ```[Frozen]``` attribute is attached to a parameter, it requests a new instance one more time and keeps using that instance for the remainder of the test. When it is time to create the AwesomeClass, and it needs an implementation of the ```IDependency1``` type, it uses the same one as the one requested as a parameter. 

> It is vital to note that the order of your parameters does count. Always place the parameters with the ```[Frozen]```  attribute first, so they surely get injected in the right places.

> TLDR: The UnitTest attribute magically builds all you ever need. When you want to interfere with the interfaces about to be injected, request this as a parameter and add the ```[Frozen]``` tag to it. Place this parameter before the parameter containing the object to be injected.

## Example with a specific setup
```cs
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
 ```
 In this example, you can see 2 stages in our "arrange" phase. The first stage sets the input parameters to a test-specific value. Doing a setup like this shows the reader of your test clearly that those values are important.

The second stage is setting up the behavior of the fake interface. It instructs that object that whenever a call to "Result" is made with the value of "withThis", it should return the value 5.

# Packages used in these examples
To create a setup like this, you need some nuget packages. These can be installed in Visual Studio with the following package manager commands:
```
Install-Package xunit
Install-Package xunit.runner.visualstudio
Install-Package FakeItEasy
Install-Package AutoFixture
Install-Package AutoFixture.Xunit2
Install-Package AutoFixture.AutoFakeItEasy
Install-Package FluentAssertions
```
If you don't like the ```result.Should().Be("8");``` type of assertions, you can skip the FluentAssertions Library.

# That's all folks
Whenever I need to write unit tests, I always use this setup. It gives me lots of tranquility to know I'm able to change my dependencies without a headache. The bonus of small, readable unit tests is an absolute win for me as well. I hope these insights and code patterns can be useful to you as well.

In another blog post, I will go deeper into this system, and explain how you can customize the fixture to create standard functionality for each unit test without repeating yourself.

You can find [the source of these examples on my Github](https://github.com/johnverbiest/johnverbiest.github.io/tree/master/_sources/Maintainable-Unit-Testing) page.