
#!/bin/bash
sudo yum update -y
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
sudo yum install dotnet-sdk-3.1 -y
sudo yum install git -y
git clone https://github.com/bohdanD/simple-ddos.git
cd simple-ddos
dotnet build
dotnet run