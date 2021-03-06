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

#### Licence
The project is licensed under the [Apache License, Version 2.0](LICENSE).

#### History
#####2016-07-07 / v1.0.4 / DAK
- Fixed bug that prohibited proper rescheduling of tasks after completion.
#####2016-04-17 / v1.0.3 / DAK
- Updated reference for [NerdyDuck.CodedExceptions](../NerdyDuck.CodedExceptions) to v1.2.1.
- Updated reference for [NerdyDuck.Collections](../NerdyDuck.Collections) to v1.0.3.
- Switched internal error codes from integers to `ErrorCodes` enumeration.
- Universal project compiled against Microsoft.NETCore.UniversalWindowsPlatform 5.1.0 .

#####2016-04-06 / v1.0.2 / DAK
- Updated reference for [NerdyDuck.CodedExceptions](../NerdyDuck.CodedExceptions) to v1.2.0.
- Updated reference for [NerdyDuck.Collections](../NerdyDuck.Collections) to v1.0.2.
- Added deployment project to compile all projects and create/push the NuGet package in one go. Removed separate NuGet project. Removes also dependency on NuGet Packager Template.
- Extracted file signing into its own reusable MSBuild target file.
- Extracted resource generation for desktop project into its own reusable MSBuild target file.
- Created a MSBuild target for automatic T4 transformations on build. Removes dependency on Visual Studio Modeling SDK.
- Fixed bug in Package.tt so content files are included into NuGet package.

##### 2016-03-07 / v1.0.1 / DAK
- Switched `IXmlSerializable` interfaces in `Schedule`, `ScheduledTask(T)` and `ScheduledActionBase` from public access to explicit interface implementation, where appropriate.

##### 2016-02-26 / v1.0.0 / DAK
- First release.
