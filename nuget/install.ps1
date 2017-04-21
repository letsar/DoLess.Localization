param($installPath, $toolsPath, $package, $project)
$item = $project.ProjectItems | where-object {$_.Name -eq "doless.localization.json"} 
$item.Properties.Item("BuildAction").Value = [int]0