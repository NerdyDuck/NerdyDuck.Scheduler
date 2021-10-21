# ![Logo](media/NerdyDuck.Scheduler.svg) NerdyDuck.Scheduler
#### A task scheduler for .NET, with time schedules similar to Windows&reg; Task Scheduler.

This project provides a set of base classes to create a scheduler hosted within an application. The actions executed by a scheduled task are tailored to the specific application. The scheduler offers different task schedules, like regular intervals, daily, weekly, monthly or one-time occurrences. The tasks are executed on the default thread pool.

#### Platforms
- .NET Standard 2.0 (netstandard2.0), to support .NET Framework (4.6.1 and up), .NET Core (2.0 and up), Mono (5.4 and up), and the Xamarin and UWP platforms.
- .NET 5 (net5.0)
- .NET 6 (net6.0)

#### Dependencies
The project uses the following NuGet packages not issued by Microsoft as part of the BCL:
- [NerdyDuck.CodedExceptions](https://www.nuget.org/packages/NerdyDuck.CodedExceptions)

#### Languages
The neutral resource language for all texts is English (en-US). Currently, the only localization available is German (de-DE). If you like to add other languages, feel free to send a pull request with the translated resources!

#### How to get
- Use the NuGet package (include debug symbol files and supports [SourceLink](https://github.com/dotnet/sourcelink): https://www.nuget.org/packages/NerdyDuck.Scheduler
- Download the binaries from the [Releases](../../releases/) page.

#### More information
For examples and a complete class reference, please see the [Wiki](../../wiki/). :exclamation: **Work in progress**.

#### License
The project is licensed under the [MIT License](LICENSE).

#### History
##### TBD / 2.0.0-rc.1 / DAK
- Upgraded platform to .NET Standard 2.0, .NET 5 and .NET 6
- Removed separate binaries for UWP (use .NET Standard 2.0 instead)
- Changed German resources from de-DE to just de.
- Restructured repository, using Directory.Build.props/.targets for common configuration
- Switched license from Apache 2.0 to MIT
- Complete unit tests added
- Some bug fixes

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
