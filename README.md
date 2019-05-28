# AwsDotNetCmdUtils
Collection of .NET command line utilities for Amazon Web Services

Installation on host computer
=============================
1. .NET Core 2.2 - download runtime 2.2.5 from https://dotnet.microsoft.com/download/dotnet-core/2.2
2. AwsDotNetCmdUtils - download release ZIP file from GitHub and extract to a suitable directory on your PC - i.e. C:\Program Files (x86)\wpqs\AwsDotNetCmdUtils
3. AWS CLI - download the Amazon CLI for Windows (64-bit) from https://docs.aws.amazon.com/cli/latest/userguide/install-windows.html

AwsDotNetS3LargeFileXferCmd
===========================
Transfers large files to and from S3 storage.

Run Information
---------------

1. use the ASW console ( https://console.aws.amazon.com/console) to create a S3 bucket and record 
	bucket name, region name
	record key and secret key needed to access it (best to create AIM keys rather than use root keys)
2. open a Command Prompt window and issue the following commands:
    a. cd c:\users\wills\AwsDotNetCmdUtils
    b. aws config
	     (enter region name and keys when prompted)
    c. dotnet AwsDotNetS3LargeFileXferCmd --operation upload --inputfile c:\users\wills\largefile.bin --bucketname novastor-nuc-test --bucketregion us-east-1
	   dotnet AwsDotNetS3LargeFileXferCmd --operation download --outputfile c:\users\wills\largefile.bin --bucketname novastor-nuc-test --bucketregion us-east-1

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

