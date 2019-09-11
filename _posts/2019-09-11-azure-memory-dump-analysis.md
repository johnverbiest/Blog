---
title: Analyzing website memory dumps
categories: [code, code analysis, azure, windbg]
tags: [code, analysis, legacy, memdump, windbg]
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