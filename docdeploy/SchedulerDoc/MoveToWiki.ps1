# ******************************************************************************
# <copyright file="MoveToWiki.ps1" owner="Daniel Kopp">
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
# <file name="MoveToWiki.ps1" date="2015-09-16">
# Copies the generated class reference documentation into the wiki repo,
# while flattening the directory structure, omitting manually edited files,
# and changing the links to media files to adapt to the flattened structure.
# </file>
# ******************************************************************************

# Target Path
$WikiPath = "..\..\..\NerdyDuck.Scheduler.wiki\"
# Source paths
$HelpPath = ".\Help\"
$MediaPath = ".\Help\Media\"

#First, copy images in media path
Copy-Item -Path ([System.IO.Path]::Combine($MediaPath, "*.*")) -Destination $WikiPath -Recurse

#Then iterate through the doc files
Get-ChildItem $HelpPath | ForEach-Object {
	if ($_.Extension -eq ".md") {
		# Markdown files; Skip Home and _Sidebar, as they are manually maintained
		if ($_.Name -ine "Home.md" -and $_.Name -ine "_Sidebar.md") {
			# Remove media directory from all paths
			Get-Content $_.FullName | ForEach-Object {
				$_ -replace "\(media/", "("
			} | Set-Content ([System.IO.Path]::Combine($WikiPath, $_.Name))
		}
	}
	else
	{
		# Non-Markdown files; copy, if not a directory
		if ($_.Attributes -ine [System.IO.FileAttributes]::Directory) {
			Copy-Item -Path $_.FullName -Destination $WikiPath
		}
	}
}
