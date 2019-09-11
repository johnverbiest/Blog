---
title: Analyzing website memory dumps
categories: [code, code analysis, azure, windbg, dotMemory]
tags: [code, analysis, legacy, memdump, windbg, dotMemory]
---
![Bug](/assets/azure-memdump.png#rightIcon)
When debugging a production website, Application Insights sometimes is not enough, and you need more powerful tools.
<!--more-->

Going through a memory dump is both a blessing and a curse. To be fairly honest, it is mostly a curse.
Today one of our services experienced multiple "OutOfMemoryExceptions". Instead of doing what we mostly do, restarting the hell out of that server, we were a bit smarter and asked Azure to give us a memory dump. The memory dump led us to a specific web request causing a quasi-memory-leak by putting lots of objects (530 038 to be exact) in memory. What follows is how we did it.

# Step one: Getting the memory dump
## Identifying the correct instance
Our service runs on multiple instances, and we needed the memory dump for that specific instance. You can find the right instance easily by going to the live metrics stream of the connected Application Insights. 

![Application Insights](/assets/azure-memdump-ai.png)

In case of this screenshot, the instance name is RD0003FF22484C, but that will be different for your case. 
There are other ways of detecting what instance is the naughty one. For example, you can use the application insights search option, or any other way to find the instance name

## Requesting the memory dump
You should request the memory dump while the mischief is happening. A memory dump is a snapshot of threads and memory of the running web app and does not contain historical data. Getting a memory dump when the error is no longer happening is pointless.

To request the memory dump, you need to open the web app diagnostic tools. To do this, you open your Web App page in Azure, click "Diagnose and solve problems", and click "Diagnostic Tools."

![Path to diagnositc tools](/assets/azure-memdump-path-to-diagnostic-tools.png)

The App Service Diagnostics Tools and Resources house a few handy resources. The ones I use most are the "Collect Memory Dump", "Collect .NET Profiler Trace" and "Advanced Application Restart". All of these tools prove to be very handy from time to time, and one should keep them in mind at all times when your application is doing the dirty thing.

![Azure Diagnostic tools](/assets/azure-memdump-diagnostic-tools.png)

As you can guess, the tool we are going to use is going to be the "Collect Memory Dump" one. After we successfully created a memory dump, we can restart only the one specific bogus instance using "Advanced Application Restart". Doing so limits the impact of the restart.

![Memdump starting](/assets/azure-memdump-start-memdump.png)

Starting the memory dump is as easy as selecting the instance you want the dump for, clicking "Collect Memory Dump" and wait until all three steps are done. 
In mode, you can opt to skip the "Analyze Data" step; I tend not to do it because all inputs of data analysis are helpful. 

Creation of a memory dump can take a while, depending on how much memory your application is munching. In my case, the dump was about 3GB big and took quite a while to create.

![Download Memory Dump](/assets/azure-memdump-memdump-done.png)

You can open the "Azure analysis" on the right of the screen. The dumps themselves show on the right. For our next steps, we are going to need the dump on our hard drive, so download away.

# Step two: getting more tools
For my analysis today, I made use of two tools. The first tool was [JetBrains dotMemory](https://www.jetbrains.com/dotmemory/), and the other one is [WinDbg](https://docs.microsoft.com/en-us/windows-hardware/drivers/debugger/debugger-download-tools). At last, I've downloaded an extension called [SOSex](http://www.stevestechspot.com/). 

My experience with these tools is that dotMemory is a friendly general analysis tool to pinpoint problems generally. With WinDbg, you can dig deep into the data, but it is hard to have a general overview of what is happening.

Below is a screenshot of both dotMemory and WinDbg and the difference in the user interface is very clear. 

![dotMemory vs windbg](/assets/azure-memdump-dotmemory-vs-windbg.png)

To be specific: dotMemory is on the left of the red line, WinDbg on the right.

# Step 3: General analysis with dotMemory
To open our memory dump, click "Import Dump", select the correct file, and click "Open".

![open dotMemory](/assets/azure-memdump-open-dotMemory.png)

It will show some screens and do some initial analytics. After you went through them you should see something like this:

![Initial analytics](/assets/azure-memdump-dotmemory-initial.png)

It was quite obvious there was a problem. There is 450.49 MB worth of string data in memory. Another problem that pops up is that three objects in memory hold about 415.96MB of data. That can't be a coincidence.

![Drilling down](/assets/azure-memdump-dotmemory-biglist.png)

By clicking the "3xEnumerable...", followed by opening one of those three, I got the data above. In the list there are 530 038 items, accounting to more than 146Mb of data for a single API call. Adding the knowledge that at the time this memory dump was taken, there were three of those lists; there could be only one conclusion. This list must be the cause of the problem. 

Sadly dotMemory did not allow me to pinpoint what call caused this situation, so a deeper dive had to be taken, straight into the deep waters of WinDbg.