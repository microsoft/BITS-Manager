
$csprojxml=[xml](get-content "..\BITSManager.csproj")
$propertyGroups=$csprojxml.Project.PropertyGroup
$csprojVersion = ""
Foreach ($propertyGroup in $propertyGroups)
{
    $version = "$($propertyGroup.ApplicationVersion)"
	if ($version -ne "")
	{
		$csprojVersion = $version -replace "%2a", "0"
		write-host "Extracted csproj version=$($csprojVersion)"
	}
}

if ($csprojVersion -ne "")
{
	$wixxml=[xml](get-content ".\BITSManager.wxs")
	$wixVersion=$wixxml.Wix.Product.Version
	write-host "Extracted wix version=$($wixVersion)"

	$newGuid="$([guid]::NewGuid())"
	write-host "New wix guid=$($newGuid)"

	$wixxml.Wix.Product.Version=$csprojVersion
	$wixxml.Wix.Product.ID=$newGuid
	
	$outfile = "$($(get-location).Path)\BITSManager.wxs"
	write-host "Writing to file $($outfile)"
	$wixxml.Save($outfile)
}
