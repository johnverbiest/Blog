---
title: My first babysteps in static code analysis with nDepend
categories: [code, code analysis]
---
# Introduction
![Sherlock Holmes](images/sherlock-holmes.png#right)
A few weeks ago, I was asked to look into static code analysis, mainly with the tool
nDepend. I had never had any experience with static code analysis yet, and was fairly
curious about what this was all about. 

In this blogpost I will be talking about what static code analysis is, what the 
difference is with dynamic code analysis and when to use it. After that I will be 
talking about some metrics and conclusions you can get from a static code analysis
tool and I will be ending my post with a small overview what nDepend has to offer.

# Static code analysis
## What is it?
You might not know it, but in most IDE's we already use static code analysis all the
time. However wrapped into a nice package known as IDE features. 

![Static code analysis ide features](images/static-code-analysis-ide.png)

As you can see in the image above, I can get 4 kinds of static code analysis from
this small piece of software alone. 
 - On top we have codelens, informing us how many times and where the function is used
 - The blue squiggly line indicates we have a local variable that does not comply to the local coding conventions
 - The red squiggly line indicates we should expect a build error on that line
 - The slightly dimmed code below tells us that code is actually unreacheable.

## Static vs Dynamic code analysis
We can define the difference between static and dynamic code analysis as follows:

| Static code analysis | Dynamic code analysis|
|----------------------|----------------------|
| Code is not running  | Code is in a running state |
| Analysing code itself | Analysing features |
| For obvious probles & code smells | For everything else |
| Examples: Code style & smells, metrics, ... | Examples: Memory & CPU pressure, Debugging, ... |

In short: Static code analysis focusses most on code quality in the domain of 
maintainability & readability. Dynamic code analysis focusses in the field of
features and performance.

# Static code metrics
In this chapter i will focus on the four metrics I think are the most important 
ones for your code. I might update some new metrics later, but in my humble opinion
these are key for your code.

## Cyclomatic Complexity
Quite a mouthful as a word. When this metric is measured, it will count every ```if```, ```else```, ```switch```, ```goto```, ```for```, ```while``` and every
other known code-flow-changing keyword. This count results in the Cyclomatic 
Compexity.

To make it a little more easy to understand: Cyclomatic complexity measures the
amount of code paths in a method.

It is a good practice to keep your complexity under 10. Methods between 10 and 20
can exists but should be rather rare. Methods with a complexity of 20 and above 
should be refactored by the earliest convenience. 

When you request the metric to be displayed, it shows up like this:

![Cyclomatic Complexity](images/static-code-analysis-cyclomatic-complexity.png)

The first time I saw this one, I was like: what am I looking at? Ain't nobody can
read that. But I got it figured out, and this is how it works: 

### Reading an nDpend metrics chart
#### Settings on top and to the right
![Settings](images/static-code-analysis-metrics-top.png)

On the top you will find the following:
| Item | What it controls |
|------|------------------|
| Level | The smalles part of the chart you will see, and subsequently the metrics that are available |
| Size | The metric controlling the size of the boxes |
| Color | The metric controlling the color of the boxes |

On the left you will find a slider and some minor settings, controlling what level
of the `Color` metric correspondents with what color.