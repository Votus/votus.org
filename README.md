Large scale community task management
============================================

Helping facilitate positive change within large communities.

##How to Contribute

1. [Fork](https://help.github.com/articles/fork-a-repo) the votus.org repo
2. Make changes in your fork
3. Submit a pull request

##Getting Started

1. Clone the repo locally

    `git clone https://github.com/Votus/votus.org.git --recursive`

2. Update your remotes to push to your fork

    `git remote set-url --push origin https://github.com/username/votus.org.git`

##Build and Unit Test
**Prerequisites**
- [Web Platform Installer 4.6](http://www.microsoft.com/web/downloads/platform.aspx)
- Visual Studio 2013 [w/ NuGet Package Restore](http://docs.nuget.org/docs/workflows/using-nuget-without-committing-packages) enabled

Once you have installed these and cloned the repo locally, run the following (as Administrator):

    .\votus.org>Ci.bat

To see what else the continuous integration script can do:

    .\votus.org>Ci.bat -docs

Note: The CI script may automatically install prerequisite software needed to run, which may reboot your computer.

##Deploy and Integration Test
**Prerequisites**
- An Azure subscription [w/ Local Git deployment](http://www.windowsazure.com/en-us/develop/net/common-tasks/publishing-with-git) configured
- [Download and Import](http://msdn.microsoft.com/en-us/library/dn385850.aspx) your Azure Publish Settings

**Run it!**

    .\votus.org>Ci.bat FullCI

##Recommended Tools

1. [posh-git](https://github.com/dahlbyk/posh-git)
1. [ReSharper](http://www.jetbrains.com/resharper/download/)
2. [Git Extensions](https://code.google.com/p/gitextensions/)
3. [Specflow for Visual Studio 2013](http://visualstudiogallery.msdn.microsoft.com/9915524d-7fb0-43c3-bb3c-a8a14fbd40ee)
4. [xUnit Test Runner for Visual Studio 2013](http://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099)
5. [Git Source Control Provider for Visual Studio](http://visualstudiogallery.msdn.microsoft.com/63a7e40d-4d71-4fbb-a23b-d262124b8f4c)
