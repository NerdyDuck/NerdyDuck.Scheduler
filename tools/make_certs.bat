# make_certs.bat
# Creates a PFX file to sign the Universal Windows Unit Test App, if the file does not exist.
# Create a strong name key file (.snk) in the solution folder to sign all assemblies, if the file does not exist.
# Copyright 2015-2016 Daniel Kopp
# Licensed under the Apache License, Version 2.0

@SET PfxPath="..\TemporaryKey.pfx"
@SET SnkPath="..\NerdyDuck.snk"

@IF "%VS140COMNTOOLS%"=="" goto error_no_VS140COMNTOOLSDIR
@CALL "%VS140COMNTOOLS%vsvars32.bat"

@IF "%WindowsSdkDir%"=="" goto error_no_WindowsSdkDir
@IF "%WindowsSDK_ExecutablePath_x86%"=="" goto error_no_WindowsSDK_ExecutablePath_x86

@IF EXIST %PfxPath% goto skip_PFX
@SET FileBase=%TEMP%\cert%RANDOM%
@SET YearBegin=%date:~-4%
@SET /a YearEnd=%YearBegin% + 4

@"%WindowsSdkDir%bin\x86\makecert.exe" -n "cn=%USERNAME%" -r -a sha256 -len 2048 -sv "%FileBase%.pvk" "%FileBase%.cer" -b 01/01/%YearBegin% -e 12/31/%YearEnd%
@if not errorlevel 0 goto error_Makecert

@"%WindowsSdkDir%bin\x86\pvk2pfx.exe" -pvk "%FileBase%.pvk" -spc "%FileBase%.cer" -pfx "%FileBase%.pfx" -po ""
@if not errorlevel 0 goto error_Pvk2pfx

@copy /y "%FileBase%.pfx" %PfxPath%
@if not errorlevel 0 goto error_CopyPFX

@del "%FileBase%.*"
@goto create_SNK

:skip_PFX
@echo PFX file for universal Windows Test App (%PfxPath%) already exists.
@goto create_SNK

:create_SNK
@IF EXIST %SnkPath% goto skip_SNK
@"%WindowsSDK_ExecutablePath_x86%sn.exe" -q -k 2048 %SnkPath%
@if not errorlevel 0 goto error_SNK
@echo The strong name key was generated, but you still need to modify the
@echo InternalsVisibleToAttribute in the AssemblyInfo.cs files of the
@echo libraries to match the new key.
@goto end

:skip_SNK
@echo Strong name key file (%SnkPath%) already exists.
@goto end

:error_no_VS140COMNTOOLSDIR
@echo ERROR: Cannot determine the location of the VS Common Tools folder.
@goto end

:error_no_WindowsSdkDir
@echo ERROR: Cannot determine the location of the Windows Toolkit folder.
@goto end

:error_no_WindowsSDK_ExecutablePath_x86
@echo ERROR: Cannot determine the location of the Windows SDK Executable Path.
@goto end

:error_Makecert
@echo ERROR: Cannot create certificate for Windows Test App
@goto end

:error_Pvk2pfx
@echo ERROR: Cannot create PFX file for Windows Test App
@goto end

:error_CopyPFX
@echo ERROR: Cannot copy PFX file into Windows Test App project folder
@goto end

:error_SNK
@echo ERROR: Cannot create strong name key file in solution folder
@goto end

:end