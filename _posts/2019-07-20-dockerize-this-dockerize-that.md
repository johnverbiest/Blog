---
title: Dockerize this Dockerize That - Build and deploy for several types of environments
categories: [devops, build and deploy]
tags: [devops, azure, docker, container, nodejs, dotnet core, angular]
---
![Docker](/assets/docker-on-the-sea.jpg#rightIcon)
I recently inherited a software development project from a client of mine.  
This software was what buggy and they wanted me to host the software, and continue writing code on it. 
The code I received consist of two parts: one part was Nodejs, and one part Angular. Although I have some experience with Angular, my knowledge of Nodejs is quite limited, to say the least. For hosting, I have some Linux-servers where I only have limited access to, but not sufficient access to enable Nodejs. I need a solution: Docker.
<!--more-->

If you don't want the story behind my docker choices, but want to skip to how I did it, don't read the Introduction part.

# Introduction
## Third one is the charm
By the time I received the full code of the software, I already made a third component that did some reporting. Doing what I like best, I picked the technologies I'm most familiar with and went ahead in a Dotnet Core 2.1 Web API with a React frontend. Choosing Core turned out to be a great idea later because I did not know at the time how and where my hosting needs would emerge.

## Three technology types, one tiny hosting budget
So there I was, sitting at my laptop, having never built a Nodejs application before, and no idea how to host it. To add to the problem, I would have to host the webserver, database, DNS, and buy SSL keys for less than 100 euro per month. 

## Then there was Docker
I quickly discovered that the cheapest way to host something on server is using a Linux machine. However, I never used Docker before, so this would be exciting.

## So what is Docker anyway?
Docker is a standardised environment to build and deploy applications. It comes in two flavours: Linux and Windows, but Linux is the default setting. All Microsoft Azure web apps support both Linux and Windows Docker images to run the application. 
You can run Docker on your machine, mimicking the build and production server locally with almost no difference at all. 

# Some things I did not know about Docker
I knew Docker had something to do with containers, but that is all I knew. Here's what I've learned.

## Docker is more than a running environment
My first misconception was that Docker was just a way of standardising the way you run your application, but I was very wrong. It turns out that when you want to make an image (more on images in the next chapter), you use Docker as your build server. 
The concept of this is quite remarkable: Docker provides a stable build environment wherever you are, and the build output can run anywhere in the docker application. 

## Images? What the hell?
The output of a Docker build are images. An image is a state your DDocker was at any state of your build. If your Dockerfile (this is a file containing the build and deploy steps) contains 10 steps, 10 images get generated. For running the application, you only need the last one, but the others are useful as well, more about this later.
The difference between a container and an image is this: an image is an application you want to run (binaries), and the container is the environment the application is running in. 

# Docker in practice
## Azure DevOps yaml build file
I'm a big fan of Azure DevOps, and I like to use the hosted build servers they freely provide to do all my build and deploy work. For any Linux based docker file, this is the way I configure builds:

```yaml
trigger:
  - master

pool:
  name: Hosted Ubuntu 1604

steps:
  - script: printenv
    displayName: 'Print environment variables'

  - task: Docker@0
    displayName: 'Build an image'
    inputs:
      azureSubscription: 'AzureSubscriptionName'
      azureContainerRegistry: '{"loginServer":"containerRegistry", "id" : "ContainerRegistryId"}'

  - task: Docker@0
    displayName: 'Push an image'
    inputs:
      azureSubscription: 'AzureSubscriptionName'
      azureContainerRegistry: '{"loginServer":"containerRegistry", "id" : "ContainerRegistryId"}'
      action: 'Push an image'
```

If you are working with Docker, you need a place to store the docker images. Such a place is called a Container Registry, and Azure has a solution for this: [Azure Container Registry](https://azure.microsoft.com/en-in/services/container-registry/).

In the build, you need to specify this Registry. First of all, you need a service connection to the Azure Subscription you use for this Registry and note the name both times in the "azureSubscription" part of the Docker task. The azureContainerRegistry is a bit more complicated but not hard at all. The JSON that goes in there goes like this: 

```
{"loginServer":"<Login Server>", "id" : "/subscriptions/<Subscription ID>/resourceGroups/<Resource group>/providers/Microsoft.ContainerRegistry/registries/<Container registry name>"}
```

All the information between the <> is to be updated to the correct values. To find the values, you go to the overview page of your container registry. For more information about yaml build files, go to [the YAML schema reference](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema).
