[void] [System.Reflection.Assembly]::LoadWithPartialName('System.Web')

#Create the index file containing all names of XMLs in sub-folders
function RegisterAllXmls ([string]$destDir)
{
	$destDirItem = Get-Item $destDir
	[string]$destSubDirName = $destDirItem[0].Name
	[string]$destDirAbsolute = $destDirItem[0].FullName
	[string]$indexFile = $destDirAbsolute + "\index.txt"
	[string]$downloadIndexFile = $destDirAbsolute + "\downloads.csv"
	Write-Host $indexFile
	Write-Host $downloadIndexFile
	if ((Test-Path $indexFile) -eq $true) { Remove-Item $indexFile }
	if ((Test-Path $downloadIndexFile) -eq $true) { Remove-Item $downloadIndexFile }
	"""DownloadUrl"",""FilePath"",""FileName"",""MD5-Hash""" | Out-File -FilePath $downloadIndexFile -Encoding utf8 -Append
	$xmls = dir -recurse "$destDir\*.xml"
	#$xmls | ft -group {} Name, FullName, DirectoryName
	$xmls | Foreach-Object{
		$DestFullName = $_.FullName
		$DestFullNameRelatively = $_.FullName.SubString($destDirAbsolute.Length)
        $FileHash = Get-FileHash $_.FullName -Algorithm MD5
		".$DestFullNameRelatively" | Out-File -FilePath $indexFile -Encoding utf8 -Append
		"""" + 'https://raw.githubusercontent.com/kohelet-net-admin/zefania-xml-bibles/master/' + $destSubDirName + [System.Web.HttpUtility]::UrlPathEncode($DestFullNameRelatively.replace('\', '/')) + """,""." + $DestFullNameRelatively + """,""" + $_.Name + """,""" + $FileHash.Hash + """" | Out-File -FilePath $downloadIndexFile -Encoding utf8 -Append
	}
	#Write-Warning ".$DestFullNameRelatively"
	#Write-Host $DestFullName
}

### EXTRACT ALL BIBLES ###
$JewishBibleXmlDestinationDir = "..\Zefania-Xml-Bibles\Bibles"
RegisterAllXmls "$JewishBibleXmlDestinationDir"

### EXTRACT ALL DICTIONARIES ###
$JewishBibleXmlDestinationDir = "..\Zefania-Xml-Bibles\Dictionaries"
RegisterAllXmls "$JewishBibleXmlDestinationDir"

### CREATE FULL INDEX
.\0110_RegisterZefaniaXmlFiles\bin\ZefaniaXmlTools.exe ..\Zefania-Xml-Bibles\Bibles
