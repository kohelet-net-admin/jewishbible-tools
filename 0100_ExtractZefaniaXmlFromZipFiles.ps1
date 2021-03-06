[void] [System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')

function Expand-ZIPFile($file, $destination)
{
  $encoding = [System.Text.Encoding]::GetEncoding(437) # Zefania XML is CP437 encoded (not UTF-8)
  [System.IO.Compression.ZipFile]::ExtractToDirectory($file, $destination, $encoding)
}

function ExpandAllZips ([string]$srcDir, [string]$destDir)
{
	$srcDirItem = Get-Item $srcDir
	if ((Test-Path -PathType Container $destDir) -eq $true) 
	{ 
		#Cleanup old work folder structure
		Write-Warning "Old workspace at $destDir will be cleaned up, now"
		Remove-Item $destDir -Recurse -Force -Confirm:$true
	} 
	if ((Test-Path -PathType Container $destDir) -eq $false) 
	{ 
		$destDirItem = New-Item -ItemType Directory -Force -Path $destDir
	}
	else
	{
		$destDirItem = Get-Item $destDir
	}
	[string]$srcDirAbsolute = $srcDirItem[0].FullName
	[string]$destDirAbsolute = $destDirItem[0].FullName
	#Write-Host $srcDirItem
	#Write-Host $srcDirAbsolute 
	#Write-Host $srcDir
	#Write-Host $destDir
	$zips = dir -recurse "$srcDir\*.zip"
	#$zips | ft -group {} Name, FullName, DirectoryName
	$zips | Foreach-Object{
		$SrcName = $_.Name
		$SrcFullName = $_.FullName
		[string]$SrcFullNameRelatively = $_.FullName.SubString($srcDirAbsolute.Length)
		#$DestFullNameRelatively = $SrcFullNameRelatively.substring(0, $SrcFullNameRelatively.length-4)
		#$DestFullNameRelatively = (get-item $SrcFullName).Directory.FullName
		$DestFullNameRelatively = $SrcFullNameRelatively.substring(0, $SrcFullNameRelatively.lastindexof("\"))
		$DestFullName = $destDirAbsolute + $DestFullNameRelatively
		#Write-Host $SrcFullName
		#Write-Host $SrcFullNameRelatively
		#Write-Warning "$DestFullNameRelatively"
		Write-Host $DestFullName
		Expand-ZIPFile $SrcFullName $DestFullName
	}
}

### EXTRACT ALL BIBLES ###
$ZefaniaXmlSourceDir = ".\Zefania XML Sources\Bibles"
$JewishBibleXmlDestinationDir = "..\Zefania-Xml-Bibles\Bibles"
ExpandAllZips "$ZefaniaXmlSourceDir" "$JewishBibleXmlDestinationDir"

### EXTRACT ALL DICTIONARIES ###
$ZefaniaXmlSourceDir = ".\Zefania XML Sources\Dictionaries"
$JewishBibleXmlDestinationDir = "..\Zefania-Xml-Bibles\Dictionaries"
ExpandAllZips "$ZefaniaXmlSourceDir" "$JewishBibleXmlDestinationDir"
