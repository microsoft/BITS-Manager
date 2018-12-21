# PowerShell file to rebuild the BITS Manager installer BITSManager.msi
#
# This file will first "stamp" the installer with the BITS Manager AssemblyVersion
#     which is taken from AssemblyInfo.cs and put into the .wxs file
#
# The installer toolkit used is WIX, a common open-source
# Windows installer. This file assumes that you have version 3.11.1
# installed from https://github.com/wixtoolset/wix3/releases/tag/wix3111rtm
#
# Learn more about WiX from https://wixtoolset.org
#


$infile="..\BITSManager\Properties\AssemblyInfo.cs"
$regex = '^\[assembly:\s*AssemblyVersion\("(\d*\.\d*\.\d*\.\d*)"\)'
$results = select-string -Path $infile -Pattern $regex
$assemblyVersion = $results.Matches.Groups[1].Value

if ($assemblyVersion -ne "")
{
	$wixxml=[xml](get-content ".\BITSManager.wxs")
	$wixVersion=$wixxml.Wix.Product.Version
	write-host "Extracted original wix version=$($wixVersion)"
	write-host "New wix version=$($assemblyVersion)"

	$wixxml.Wix.Product.Version=$assemblyVersion
	
	$outfile = "$($(get-location).Path)\BITSManager.wxs"
	write-host "Writing to file $($outfile)"
	$wixxml.Save($outfile)
}
else
{
	write-error "ERROR: unable to get assembly version number"
}


# Make the installer programing using the WiX Toolset.
# The version 3.11 was the most recent when this sample was made.

$WIXDIR="c:\Program Files (x86)\WiX Toolset v3.11\bin"
& $WIXDIR"\candle.exe" BITSManager.wxs
& $WIXDIR"\light.exe" BITSManager.wixobj
del BITSManager.wixobj
del BITSManager.wixpdb