# NerdyDuck.Scheduler
#### A task scheduler for .NET 4.6+ and UWP, with time schedules similar to Windows&reg; Task Scheduler.

This project provides a set of base classes to create a scheduler hosted within an application. The actions executed by a scheduled task are tailored to the specific application. The scheduler offers different task schedules, like regular intervals, daily, weekly, monthly or one-time occurrences. The tasks are executed on the default thread pool.

#### Platforms
- .NET Framework 4.6 or newer for desktop applications
- Universal Windows Platform (UWP) 10.0 (Windows 10) or newer for Windows Store Apps and [Windows 10 IoT](https://dev.windows.com/en-us/iot)

#### Dependencies
The project uses the following NuGet packages that are either found on NuGet.org or my own feed (see below):
- NerdyDuck.CodedExceptions
- NerdyDuck.Collections

#### Languages
The neutral resource language for all texts is English (en-US). Currently, the only localization available is German (de-DE). If you like to add other languages, feel free to send a pull request with the translated resources!

#### How to get
- Use the NuGet package from my [MyGet](https://www.myget.org) feed: [https://www.myget.org/F/nerdyduck-release/api/v3/index.json](https://www.myget.org/F/nerdyduck-release/api/v3/index.json). If you need to debug the library, get the debug symbols from the same feed: [https://www.myget.org/F/nerdyduck-release/symbols/](https://www.myget.org/F/nerdyduck-release/symbols/).
- Download the binaries from the [Releases](../../releases/) page.
- You can clone the repository and compile the libraries yourself (see the [Wiki](../../wiki/) for requirements).

#### More information
For examples and a complete class reference, please see the [Wiki](../../wiki/). :exclamation: **Work in progress**.

#### History
##### 2016-03-07 / v1.0.1 / DAK
- Switched `IXmlSerializable` interfaces in `Schedule`, `ScheduledTask&lt;T&gt;` and `ScheduledActionBase` from public access to explicit interface implementation, where appropriate.

##### 2016-02-26 / v1.0.0 / DAK
- First release.
