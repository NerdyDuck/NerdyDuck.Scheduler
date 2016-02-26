# ******************************************************************************
# <copyright file="MakeReleaseZip.ps1" owner="Daniel Kopp">
# Copyright 2015-2016 Daniel Kopp
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# </copyright>
# <author name="Daniel Kopp" email="dak@nerdyduck.de" />
# <file name="MakeReleaseZip.ps1" date="2015-09-16">
# Creates a zip file from the release folders of the SchedulerDesktop and
# SchedulerUniversal projects, versioning the file using the
# InformationalVersionAttribute in the AssemblyInfo.cs file.
# </file>
# ******************************************************************************

# Target Path
$OutputPath = ${env:USERPROFILE} + "\Documents\"
# Source paths
$AssemblyInfoPath = "..\src\SchedulerUniversal\Properties\AssemblyInfo.cs"
$UniversalAnyPath = "..\src\SchedulerUniversal\bin\Release"
$UniversalARMPath = "..\src\SchedulerUniversal\bin\ARM\Release"
$Universalx86Path = "..\src\SchedulerUniversal\bin\x86\Release"
$Universalx64Path = "..\src\SchedulerUniversal\bin\x64\Release"
$DesktopPath = "..\src\SchedulerDesktop\bin\Release\"
$TempPath = ${env:TEMP} + "\" + [System.Guid]::NewGuid() + "\"

# Create temporary folder and copy the file structure into it.
New-Item $TempPath -ItemType Directory
Copy-Item -Path $UniversalAnyPath -Destination ($TempPath + "uap10.0\anyCPU\") -Recurse
Copy-Item -Path $UniversalARMPath -Destination ($TempPath + "uap10.0\arm\") -Recurse
Copy-Item -Path $Universalx86Path -Destination ($TempPath + "uap10.0\x86\") -Recurse
Copy-Item -Path $Universalx64Path -Destination ($TempPath + "uap10.0\x64\") -Recurse
Copy-Item -Path $DesktopPath -Destination ($TempPath + "net46\") -Recurse

$OutputFile = $OutputPath + "test.zip"
Get-Content $AssemblyInfoPath | ForEach-Object {
	if ($_ -match "AssemblyVersion") {
		$StartPos = $_.IndexOf("""") + 1
		$Length = $_.IndexOf("""", $StartPos) - $StartPos
		$FileVer = [System.Version]::new($_.SubString($StartPos, $Length))
		$OutputFile = $OutputPath + "NerdyDuck.Scheduler_" + $FileVer.ToString(3) + ".zip"
	}
}

Add-Type -Assembly System.IO.Compression.FileSystem
$CompressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
[System.IO.Compression.ZipFile]::CreateFromDirectory($TempPath, $OutputFile, $CompressionLevel, $false)

Remove-Item -Path $TempPath -Recurse
$OutputFile
