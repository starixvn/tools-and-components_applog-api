param([string]$ip)
if($ip -ne $null)
	{
		#Test-WSMan $ip
		Write-Host $ip " is restarting"
		Restart-Computer -computer $ip -force
	}
	else
	{
		Write-Host $ip " is null"
	}