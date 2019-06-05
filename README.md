# AwsDotNetCmdUtils
Collection of .NET command line utilities for Amazon Web Services:
* AwsDotNetS3LargeFileXferCmd


Installation on host computer
=============================
1. .NET Core 2.2 - download runtime 2.2.5 from https://dotnet.microsoft.com/download/dotnet-core/2.2
2. AwsDotNetCmdUtils - download latest [release ZIP](https://github.com/wpqs/AwsDotNetCmdUtils/releases) file from GitHub and extract to a suitable directory on your PC - i.e. C:\Program Files\WPQS\AwsDotNetCmdUtils
3. AWS CLI - download the Amazon CLI for Windows (64-bit) from https://docs.aws.amazon.com/cli/latest/userguide/install-windows.html
4. Create a test file called 'large file.bin' (500MB) in a suitable directory on your PC - i.e. 'c:\users\wills\Aws Test'
	fsutil file createnew largefile.bin 524288000
	ren largefile.bin "large file.bin"

Utilities
=========

AwsDotNetS3LargeFileXferCmd
----------------------------
Transfers large files to and from S3 storage.

Setup

1. use the ASW console ( https://console.aws.amazon.com/console) to create a S3 bucket and record - see AwsResCmds.txt
	bucket name (novastor-nuc-test), region name (us-east-1)
	record key and secret key needed to access it (best to create AIM keys rather than use root keys)
2. open a Command Prompt window and issue the following commands:
  * cd "c:\users\wills\Aws Test"
  * aws config
     (enter region name and keys when prompted)
  * enter commands - see below

Commands

  * Display help

       dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --help

  * Upload single file c:\users\wills\largefile.bin to bucket novastor-nuc-test in us-east-1

       dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --bucketregion eu-west-2 --bucketname novastor-test-0 --operation upload 'c:\users\wills\Aws Test\large file.bin'

  * Download single file large file.bin from bucket novastor-nuc-test in us-east-1 to c:\users\wills\largefile.bin

       dotnet "C:\Program Files\WPQS\AwsDotNetCmdUtils\AwsDotNetS3LargeFileXferCmd.dll" --bucketregion eu-west-2 --bucketname novastor-test-0 --operation download 'c:\users\wills\Aws Test\large file.bin' overwrite

Notes

  * Performance may be improved by changing the default values for --threads and --partialsize


Build History
=============

   * v1.1.32.1
       * improvement to UI refactor GetArgValue()
   * v1.1.32.0
       * improvements to UI and bug fixes
	   * implemented quotes so spaces can exist in filename
       * added parameters for setting changing default threads and partialsize as well as overwrite option for download 
	   * download checks folder exists and will fail if file exists (unless overwrite specified)
	   * unit tests for CmdLineParams
   * v1.1.30.0
       * initial release basic upload and download facilities for S3 bucket

Build Information
=================

Development Platform
  Visual Studio 2017, 15.9.7

Project Type
  .NetCore, Console App

Settings
  Project | Properties - Build - Advanced, Set Language version 7.1

Packages:
  1. Microsoft.NetCore.App v2.2.0
  2. AWSSDK.S3 v3.3.102.5

