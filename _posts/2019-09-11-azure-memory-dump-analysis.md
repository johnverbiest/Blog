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
