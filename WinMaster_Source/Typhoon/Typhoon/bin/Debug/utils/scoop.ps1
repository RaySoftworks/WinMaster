#choco install
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))
choco upgrade chocolatey

#scoop program list to install
scoop install git
scoop install cowsay
scoop install neofetch 

scoop update

neofetch
cowsay Installation has been finished ("4s remaining")
Start-Sleep -Seconds 4
Read-Host -Prompt "Press Enter To Exit"