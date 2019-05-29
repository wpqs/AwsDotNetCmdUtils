# AwsDotNetCmdUtils
Collection of .NET command line utilities for Amazon Web Services:
* AwsDotNetS3LargeFileXferCmd


Installation on host computer
=============================
1. .NET Core 2.2 - download runtime 2.2.5 from https://dotnet.microsoft.com/download/dotnet-core/2.2
2. AwsDotNetCmdUtils - download release ZIP file from GitHub and extract to a suitable directory on your PC - i.e. C:\Program Files\WPQS\AwsDotNetCmdUtils
3. AWS CLI - download the Amazon CLI for Windows (64-bit) from https://docs.aws.amazon.com/cli/latest/userguide/install-windows.html
4. Create a test file called largefile.bin (500MB) in a suitable directory on your PC - i.e. c:\users\wills\
	fsutil file createnew largefile.bin 524288000

AwsDotNetS3LargeFileXferCmd
===========================
Transfers large files to and from S3 storage.

Run Information
---------------

1. use the ASW console ( https://console.aws.amazon.com/console) to create a S3 bucket and record 
	bucket name (novastor-nuc-test), region name (us-east-1)
	record key and secret key needed to access it (best to create AIM keys rather than use root keys)
2. open a Command Prompt window and issue the following commands:
  * cd c:\users\wills
  * aws config
     (enter region name and keys when prompted)
  * dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --help
  * dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --bucketregion us-east-1 --bucketname novastor-nuc-test --operation upload c:\users\wills\largefile.bin
  * dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --bucketregion us-east-1 --bucketname novastor-nuc-test --operation download c:\users\wills\largefile.bin


Build Information
-----------------

Development Platform
  Visual Studio 2017, 15.9.7

Project Type
  .NetCore, Console App

Settings
  Project | Properties - Build - Advanced, Set Language version 7.1

Packages:
  1. Microsoft.NetCore.App v2.2.0
  2. AWSSDK.S3 v3.3.102.5

