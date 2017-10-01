param($installPath, $toolsPath, $package, $project)

Write-Host "init.ps1 called"

# assumes solution directory is above project
$repo = (Get-Item $project.FullName).directory.parent
$repoDir = $repo.fullname
Write-Host "Repo dir is $repoDir"

$installFilesDir = "$installPath\Files"

$cakeDir = "$installFilesDir\Build"
Write-Host "common functions are at $cakeDir"
Copy-Item $cakeDir $repoDir -Recurse -Force

function MoveFileIfNotExistsAlready{
	param($fileName)
	$source = "$installFilesDir\$fileName"
	
	$destination = "$repoDir"
	$destination = Join-Path $destination $fileName
	
	if(Test-Path $destination){
		Write-Host "File already exists at $destination"
	} else {
		Copy-Item $source $destination -ea Stop
		Write-Host "Copied $source to $destination"
	}
}

function ReplaceFile{
	param($fileName, $fileNameIfDifferentAtDestination = $null)
	$source = "$installFilesDir\$fileName"
	
	$destination = "$repoDir"
	if($fileNameIfDifferentAtDestination -eq $null){
		$destination = Join-Path $destination $fileName
	} else {
		$destination = Join-Path $destination $fileNameIfDifferentAtDestination
	}

	Copy-Item $source $destination -ea Stop
	Write-Host "Copied $source to $destination"
}

$gitIgnorePath = "$repoDir/.gitignore"
if(test-path $gitIgnorePath){
	write-host "git ignore file exists at $gitIgnorePath"
	$gitIgnoreContents = [IO.File]::ReadAllText($gitIgnorePath)
	if($gitIgnoreContents.Contains("tools/*") -eq $false){
		write-host "add tools to gitignore"
		$gitIgnoreContents += [Environment]::NewLine + "tools/*" + [Environment]::NewLine
		[IO.File]::WriteAllText($gitIgnorePath, $gitIgnoreContents)
		write-host "written to gitignore file"
	}
} else {
	write-host "add .gitignore file"
	ReplaceFile "git.ignore" ".gitignore"	
}

ReplaceFile "build.ps1"
MoveFileIfNotExistsAlready "build.cake"
MoveFileIfNotExistsAlready "cake.config"

$buildCakeFile = "$repoDir\build.cake"
$content = [IO.File]::ReadAllText($buildCakeFile)
$content = $content.Replace("REPLACE_WITH_REPO_NAME", $repo.Name)
Write-Host "Writing to file $buildCakeFile"
[IO.File]::WriteAllText($buildCakeFile, $content)

Write-Host "Finished installation"