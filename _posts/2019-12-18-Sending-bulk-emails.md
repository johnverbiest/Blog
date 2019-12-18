---
title: Sending bulk emails
categories: [code, code analysis, azure, windbg, dotMemory]
tags: [code, analysis, legacy, memdump, windbg, dotMemory]
---
![Email](/assets/pinguinmail.jpg#rightIcon)
In this world of advertisements and spam, it is getting increasingly difficult to get 
your non-advertisement content to an end-user via email. Gone are the days of just setting 
up an SMTP server and go with the flow. These days there are a lot of things you need to 
consider when setting up an email server and sending bulk emails.
<!--more-->
I want to show some of these settings and techniques with the help of the questions a spam filter ask.
 
## SPF Records: Are you allowed to send an email for example.com?
![Email](/assets/spf.png#rightIcon)

One of the things a spam filter wants to see verified is if the source mail server is allowed to send an email for the domain. Verification happens via an SPF (Sender Policy Framework) record. This record is a standard TXT record on your domain, and it looks like this:

```
v=spf1 mx ip4:193.110.252.250 a:smtpgateway001.dcstar.be include:sendgrid.net -all
```

This line is splittable into three parts: The version number, the allowed hosts, and what to do with the rest.

The first part is simple: v=spf1 is the way to go.

For the second part, there are several possibilities. All these possibilities are usable next to each other and stack on one another. A validation flow will start at the beginning of the record, and when a rule hits, it gets executed. The example above states that the following hosts are acceptable to send an email (in order of appearance):
All the MX records in this domain
The server with IP address 193.110.252.250
The IP that gets resolved behind the A-record of DNS smptgateway001.dcstar.be
And anything the domain sendgrid.net states to be a valid sender
The syntax is as follows

| Syntax | Explanation |
|--------|-------------
| ip4    | A single ipv4 address or an address range
| ip6    | A single ipv6 address or an address range
| a      | All the A records for that domain name*
| mx     | All the A records for the MX records of the domain*
| ptr    | All the servers where a reverse DNS hostname ends with the domain* (avoid)
| exists | Checks if the domain specified exists
| include | Checks if the spf record of this domain matches (something like if-pass or on-pass)
| | * if you dont specify a domain, the current domain is used
| | Source: [OpenSpf.Net](https://web.archive.org/web/20120116212843/http://www.openspf.net/SPF_Record_Syntax)

The third part is a what to do with the rest kind of thing. The "all" statement is a catch-all for all other IP addresses, and the line in front of it will tell a spam filter how to interpret it.
The following types of mechanisms exist:

| Mechanism | Explanation |
|:---------:|:------------|
| + | Pass, Default, on hit, the validation passes
| - | Fail, on hit, the validation failed (not a valid server to send emails from)
| ~ | SoftFail, on hit it tells the the spam filter to allow it but to mark it suspicious
| ? | Neutral, on hit it tells the spam filter that nothing can be said about validity
|   | Source: [OpenSpf.Net](https://web.archive.org/web/20120116212843/http://www.openspf.net/SPF_Record_Syntax)

It is good on transition to use the `~all` syntax Only once you are certain all your mail
servers are included in the spf record, you should change it to `-all`

